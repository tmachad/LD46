using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hazard : MonoBehaviour
{
    public LayerMask m_PlayerLayer;
    public float m_OnTime;
    public float m_OnTimeRandomization;
    public float m_OffTime;
    public float m_OffTimeRandomization;
    private float m_TimeRemaining;
    private bool m_IsOn;

    private void Update()
    {
        m_TimeRemaining -= Time.deltaTime;
        if (m_TimeRemaining <= 0)
        {
            if (m_IsOn)
            {
                TurnOff();
            } else
            {
                TurnOn();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerUtil.GameObjectInLayerMask(collision.gameObject, m_PlayerLayer))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.ChangeHealth(-1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (LayerUtil.GameObjectInLayerMask(collision.gameObject, m_PlayerLayer))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.ChangeHealth(-1);
        }
    }

    private void TurnOn()
    {
        m_IsOn = true;
        m_TimeRemaining = m_OnTime + Random.Range(0, m_OnTimeRandomization);
        // Trigger animation for ON state
    }

    private void TurnOff()
    {
        m_IsOn = false;
        m_TimeRemaining = m_OffTime + Random.Range(0, m_OffTimeRandomization);
        // Trigger animation for OFF state
    }
}
