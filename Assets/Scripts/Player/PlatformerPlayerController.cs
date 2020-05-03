using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float m_GroundSpeed = 3.0f;
    public float m_AirDrag = 0.1f;
    public float m_AirControl = 0.5f;
    public float m_MaxJumpHeight = 3.0f;
    public float m_MinJumpHeight = 1.0f;
    public LayerMask m_GroundLayer;
    public float m_GroundCheckDistance = 0.05f;

    [Header("Input")]
    public string m_HorizontalAxis = "Horizontal";
    public string m_JumpAxis = "Jump";
    public bool m_IgnoreInputs = false;

    [Header("Appearance")]
    public Transform m_SpritesRoot;

    // Component references
    protected Animator m_Animator;
    protected Collider2D m_Collider;
    protected Rigidbody2D m_Rigidbody;

    // State tracking variables
    private float m_HorizontalInput;
    private bool m_JumpInput;
    private bool m_Jumping;
    private bool m_JumpStillPressed;
    private bool m_Grounded;
    
    protected virtual void Awake()
    {
        // Initialize references to components
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider2D>();
        m_Rigidbody = GetComponent<Rigidbody2D>();

        if (!m_SpritesRoot)
        {
            m_SpritesRoot = transform;
        }
    }

    protected virtual void Update()
    {
        // Read input and update state
        if (!m_IgnoreInputs)
        {
            m_HorizontalInput = Input.GetAxisRaw(m_HorizontalAxis);
            m_JumpInput = Input.GetAxisRaw(m_JumpAxis) > 0;
        } else
        {
            m_HorizontalInput = 0;
            m_JumpInput = false;
        }

        UpdateAnimator();
    }

    protected virtual void FixedUpdate()
    {
        // Update player's grounded state
        bool wasGrounded = m_Grounded;
        m_Grounded = CheckIfGrounded();

        // Update player's movement
        m_Rigidbody.velocity = UpdateVelocity(m_Rigidbody.velocity, wasGrounded);

        // Update input tracking in FixedUpdate to avoid a scenario where two Updates occur
        // between FixedUpdates and make it look like the button is being held
        m_JumpStillPressed = m_JumpInput;
    }

    protected virtual bool CheckIfGrounded()
    {
        Vector2 colliderBottom = new Vector2(m_Collider.bounds.center.x, m_Collider.bounds.min.y);
        Vector2 checkBoxSize = new Vector2(m_Collider.bounds.size.x * 0.95f, m_GroundCheckDistance);
        return Physics2D.OverlapBox(colliderBottom, checkBoxSize, 0, m_GroundLayer);
    }

    protected virtual Vector2 UpdateVelocity(Vector2 velocity, bool wasGrounded)
    {
        if (m_HorizontalInput != 0)
        {
            velocity.x = m_HorizontalInput * m_GroundSpeed;

            if (!m_Grounded)
            {
                // If not grounded use the air control speed
                velocity.x *= m_AirControl;
            }

            // Update transform scale to flip player to face the direction they're moving
            Vector3 scale = m_SpritesRoot.localScale;
            if (m_HorizontalInput < 0)
            {
                // Going left, flip to face left
                scale.x = -1 * Mathf.Abs(scale.x);
            }
            else
            {
                // Going right, flip to face right
                scale.x = Mathf.Abs(scale.x);
            }
            m_SpritesRoot.localScale = scale;
        }
        else if (!m_Grounded)
        {
            // Not giving input and not grounded, so apply air drag so the player doesn't coast forever
            velocity.x *= 1 - m_AirDrag;
        }

        if (m_JumpInput && m_Grounded && !m_JumpStillPressed)
        {
            velocity.y = Mathf.Sqrt(-2 * Physics2D.gravity.y * m_Rigidbody.gravityScale * m_MaxJumpHeight);
            m_Jumping = true;
        }
        else if (!m_JumpInput && m_Jumping && velocity.y > 0)
        {
            // Stopped holding jump button while jumping and still rising
            // Apply downward force to converge on m_MinJumpHeight
            float force = -1 * m_Rigidbody.mass * Physics2D.gravity.y * m_Rigidbody.gravityScale * (1 + m_MaxJumpHeight / m_MinJumpHeight);
            m_Rigidbody.AddForce(Vector2.down * force, ForceMode2D.Force);
        }

        if (m_Grounded && !wasGrounded)
        {
            // Just hit the ground, so jump must be over
            m_Jumping = false;
        }

        return velocity;
    }

    protected virtual void UpdateAnimator()
    {
        m_Animator.SetFloat("Horizontal Speed", Mathf.Abs(m_Rigidbody.velocity.x));
        m_Animator.SetFloat("Vertical Speed", Mathf.Abs(m_Rigidbody.velocity.y));
        m_Animator.SetFloat("Horizontal Velocity", m_Rigidbody.velocity.x);
        m_Animator.SetFloat("Vertical Velocity", m_Rigidbody.velocity.y);
        m_Animator.SetBool("Grounded", m_Grounded);
        m_Animator.SetBool("Jumping", m_Jumping);
    }
}
