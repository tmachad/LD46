using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour { 

    [Header("Movement")]
    public float m_GroundSpeed = 3.0f;
    public float m_AirDrag = 0.1f;
    public float m_MaxJumpHeight = 3.0f;
    public float m_JumpDownForce = 1.0f;

    [SerializeField]
    private LayerMask m_GroundLayer;
    [SerializeField]
    private float m_GroundCheckDistance = 0.05f;

    [Header("Interaction")]
    [HideInInspector]
    public List<Breakable> m_NearbyFixables;
    public float m_FixSpeed;

    [Header("Health")]
    public int m_MaxHitPoints;
    [SerializeField]
    private int m_HitPoints;
    public float m_MaxInvulnDuration;
    private float m_InvulnDuration;
    public Vector2 m_OnHitKnockback;
    public float m_StunTime;
    private float m_StunTimeRemaining;

    private Rigidbody2D m_Rigidbody;
    private Collider2D m_Collider;
    private bool m_Grounded = false;
    private float m_JumpSpeed;
    private bool m_Jumping;
    private bool m_JumpStillPressed;

    private ParticleSystem m_ParticleSystem;
    private Animator m_Animator;

    private void Awake()
    {
        m_NearbyFixables = new List<Breakable>();
        m_InvulnDuration = 0;
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
        m_JumpSpeed = Mathf.Sqrt(-2 * Physics2D.gravity.y * m_Rigidbody.gravityScale * m_MaxJumpHeight);
    }

    private void Update()
    {
        // Handle fixing things
        bool fix = Input.GetAxisRaw("Fix") > 0;

        if (m_StunTimeRemaining > 0)
        {
            fix = false;
        } else
        {
            m_Animator.SetBool("Stunned", false);
        }

        if (fix && m_NearbyFixables.Count > 0)
        {
            m_NearbyFixables[0].Fix(m_FixSpeed * Time.deltaTime);
            m_Animator.SetBool("Fixing", m_NearbyFixables[0].IsBroken());
        } else
        {
            m_Animator.SetBool("Fixing", false);
        }

        if (m_InvulnDuration < 0)
        {
            m_Animator.SetBool("Invulnerable", false);
        }

        m_InvulnDuration -= Time.deltaTime;
        m_StunTimeRemaining -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        bool jump = Input.GetAxisRaw("Jump") > 0;
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 velocity = new Vector2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y);

        if (m_StunTimeRemaining > 0)
        {
            jump = false;
            horizontal = 0;
        }

        // Check for ground below the player
        Vector2 bottom = new Vector2(transform.position.x + m_Collider.offset.x, transform.position.y + m_Collider.offset.y - m_Collider.bounds.extents.y);
        Vector2 boxSize = new Vector2(m_Collider.bounds.extents.x * 2 * 0.95f, m_GroundCheckDistance);
        Collider2D collider = Physics2D.OverlapBox(bottom, boxSize, 0, m_GroundLayer);
        bool wasGrounded = m_Grounded;
        m_Grounded = collider != null;
        m_Animator.SetBool("Grounded", m_Grounded);


        // Move horizontally if player is pressing a button, otherwise just maintain current horizontal velocity
        if (horizontal != 0)
        {
            velocity.x = horizontal * m_GroundSpeed;

            if (horizontal < 0)
            {
                // Going left, flip sprite to face left
                transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            } else if (horizontal > 0)
            {
                // Going right, flip sprite to face right
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        } else if (!m_Grounded)
        {
            // When not giving input and not in the air apply air drag so the player doesn't sideways coast forever
            velocity.x = velocity.x * (1 - m_AirDrag);
        }

        if (jump && m_Grounded && !m_JumpStillPressed)
        {
            // Pressing jump key while grounded, start jumping
            velocity.y = m_JumpSpeed;
            m_Jumping = true;
        } else if (!jump && m_Jumping && m_Rigidbody.velocity.y > 0)
        {
            // Stopped holding the jump button while jumping and still rising, apply downward force to shorten jump
            m_Rigidbody.AddForce(Vector2.down * m_JumpDownForce * m_Rigidbody.mass);
        }

        if (m_Grounded && !wasGrounded)
        {
            // Just hit the ground, so jump must be over
            m_Jumping = false;
        }

        m_Rigidbody.velocity = velocity;
        m_JumpStillPressed = jump;
        m_Animator.SetFloat("Horizontal Speed", Mathf.Abs(velocity.x));
    }

    /// <summary>
    /// Modifies the player's health and triggers related effects.
    /// </summary>
    /// <param name="change">The amount to change the player's health.</param>
    /// <returns>True if the player's health was actually changed, false otherwise.</returns>
    public bool ChangeHealth(int change)
    {
        if (change < 0)
        {
            if (m_InvulnDuration > 0)
            {
                // Ignore damage when invulnerable
                return false;
            } else
            {
                // React to damage (knockback, etc)
                Vector2 knockback = m_OnHitKnockback;
                knockback.x *= -Mathf.Sign(m_Rigidbody.velocity.x);
                m_Rigidbody.velocity = knockback;
                m_StunTimeRemaining = m_StunTime;
                m_Animator.SetBool("Stunned", true);
                m_Animator.SetBool("Invulnerable", true);

                m_InvulnDuration = m_MaxInvulnDuration;
            }
        }
        if (change > 0 && m_HitPoints < m_MaxHitPoints)
        {
            // Got healing
        }

        int initialHP = m_HitPoints;
        m_HitPoints += change;
        m_HitPoints = Mathf.Clamp(m_HitPoints, 0, m_MaxHitPoints);

        if (m_HitPoints == 0)
        {
            // HP reached zero, do something?
        }

        return initialHP != m_HitPoints;
    }

    public void PlaySparkBurst()
    {
        Debug.Log("Playing Burst");
        m_ParticleSystem.Play();
    }
}
