using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressSource : MonoBehaviour
{
    [Header("Stress")]
    public float m_StressPerSecond;
    public bool m_Active;

    [Header("Fixing")]
    public float m_FixWork;
    public bool m_FixProgressDecays;
    [Tooltip("The work per second of fix progress lost when decay starts.")]
    public float m_FixDecayRate;
    public float m_FixDecayDelay;
    private float m_FixWorkRemaining;
    private float m_FixDecayDelayRemaining;
    public Image m_FixProgressImage;
    public LayerMask m_PlayerLayer;

    private void Update()
    {
        if (m_Active)
        {
            GameManager.Instance.m_Stress += m_StressPerSecond * Time.deltaTime;
            m_FixProgressImage.enabled = true;
        } else
        {
            m_FixProgressImage.enabled = false;
        }

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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (LayerUtil.GameObjectInLayerMask(collision.gameObject, m_PlayerLayer))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.m_NearbyFixables.Remove(this);
        }
    }

    public void Break()
    {
        m_Active = true;
        m_FixWorkRemaining = m_FixWork;
        m_FixDecayDelayRemaining = m_FixDecayDelay;
    }

    public void Fix(float work)
    {
        m_FixWorkRemaining -= work;
        m_FixDecayDelayRemaining = m_FixDecayDelay;
        if (m_FixWorkRemaining <= 0)
        {
            m_Active = false;
        }

        Debug.Log("Fixing " + work + " work of progress. " + m_FixWorkRemaining + " work remaining.");
    }
}
