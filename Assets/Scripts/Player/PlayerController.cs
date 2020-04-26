using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlatformerPlayerController { 

    
    [HideInInspector]
    public List<Breakable> m_NearbyFixables;
    [Header("Interaction")]
    public float m_FixSpeed;

    [Header("Health")]
    public int m_MaxHitPoints;
    [SerializeField]
    private int m_HitPoints;
    public float m_InvulnTime;
    public Vector2 m_OnHitKnockback;
    public float m_StunTime;

    [Header("Appearance")]
    public Color m_LightColor;
    public Color m_BodyColor;
    public SpriteRenderer[] m_LightSprites;
    public SpriteRenderer[] m_BodySprites;

    // Component references
    private ParticleSystem m_ParticleSystem;

    // State tracking variables
    private float m_InvulnTimeRemaining;
    private float m_StunTimeRemaining;
    private bool m_FixInput;
    private bool m_Fixing;

    protected override void Awake()
    {
        base.Awake();
        m_NearbyFixables = new List<Breakable>();
        m_InvulnTimeRemaining = 0;
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        foreach(SpriteRenderer s in m_LightSprites)
        {
            s.color *= m_LightColor;
        }
        foreach(SpriteRenderer s in m_BodySprites)
        {
            s.color *= m_BodyColor;
        }
    }

    protected override void Update()
    {
        base.Update();
        // Handle fixing things
        m_FixInput = Input.GetAxisRaw("Fix") > 0 && !m_IgnoreInputs;


        if (m_StunTimeRemaining <= 0)
        {
            m_IgnoreInputs = false;
        }

        if (m_FixInput && m_NearbyFixables.Count > 0)
        {
            m_NearbyFixables[0].Fix(m_FixSpeed * Time.deltaTime);
            m_Fixing = m_NearbyFixables[0].IsBroken();
        } else
        {
            m_Fixing = false;
        }

        m_InvulnTimeRemaining -= Time.deltaTime;
        m_StunTimeRemaining -= Time.deltaTime;
    }

    protected override void UpdateAnimator()
    {
        base.UpdateAnimator();
        m_Animator.SetBool("Stunned", m_StunTimeRemaining > 0);
        m_Animator.SetBool("Invulnerable", m_InvulnTimeRemaining > 0);
        m_Animator.SetBool("Fixing", m_Fixing);
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
            if (m_InvulnTimeRemaining > 0)
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
                m_InvulnTimeRemaining = m_InvulnTime;
                m_IgnoreInputs = true;
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
        m_ParticleSystem.Play();
    }
}
