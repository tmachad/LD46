using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Breakable : MonoBehaviour
{
    [Header("Fix Interaction")]
    public float m_FixWork;
    public bool m_FixProgressDecays;
    [Tooltip("The work per second of fix progress lost when decay starts.")]
    public float m_FixDecayRate;
    public float m_FixDecayDelay;
    private float m_FixWorkRemaining;
    private float m_FixDecayDelayRemaining;
    public Image m_FixProgressImage;
    public Image m_FixIndicatorImage;
    public LayerMask m_PlayerLayer;

    [Header("Events")]
    public UnityEvent m_OnBreak;
    public UnityEvent m_OnFix;

    private Animator m_Animator;
    private Collider2D m_Collider;

    private bool m_IsBroken = false;
    private bool m_IsCritical;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider2D>();
        m_IsCritical = GetComponent<StressSource>() != null;
    }

    private void Start()
    {
        m_FixProgressImage.enabled = false;
        m_FixProgressImage.fillAmount = 0;
        m_FixIndicatorImage.enabled = false;
    }

    private void Update()
    {
        if (m_FixProgressDecays)
        {
            m_FixDecayDelayRemaining -= Time.deltaTime;
            if (m_FixDecayDelayRemaining <= 0)
            {
                m_FixWorkRemaining += m_FixDecayRate * Time.deltaTime;
                m_FixWorkRemaining = Mathf.Clamp(m_FixWorkRemaining, 0, m_FixWork);
            }
        }

        m_FixProgressImage.fillAmount = 1 - m_FixWorkRemaining / m_FixWork;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerUtil.GameObjectInLayerMask(collision.gameObject, m_PlayerLayer))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.m_NearbyFixables.Add(this);
            if (IsBroken())
            {
                m_FixIndicatorImage.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerUtil.GameObjectInLayerMask(collision.gameObject, m_PlayerLayer))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.m_NearbyFixables.Remove(this);
            m_FixIndicatorImage.enabled = false;
        }
    }

    public void Break()
    {
        m_FixWorkRemaining = m_FixWork;
        m_FixDecayDelayRemaining = m_FixDecayDelay;
        m_FixProgressImage.fillAmount = 0;

        if (m_Collider.IsTouchingLayers(m_PlayerLayer))
        {
            m_FixIndicatorImage.enabled = true;
        }

        m_FixProgressImage.enabled = true;
        m_IsBroken = true;
        m_Animator.SetBool("Broken", true);
        m_OnBreak.Invoke();
    }

    public void Fix(float work)
    {
        m_FixWorkRemaining -= work;
        m_FixDecayDelayRemaining = m_FixDecayDelay;

        if (m_FixWorkRemaining <= 0)
        {
            m_FixProgressImage.enabled = false;
            m_FixIndicatorImage.enabled = false;
            m_IsBroken = false;
            m_Animator.SetBool("Broken", false);
            m_OnFix.Invoke();
        }
    }

    public bool IsBroken()
    {
        return m_IsBroken;
    }

    public bool IsCritical()
    {
        return m_IsCritical;
    }
}
