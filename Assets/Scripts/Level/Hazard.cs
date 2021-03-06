﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Hazard : MonoBehaviour
{
    public LayerMask m_PlayerLayer;
    public float m_OnTime;
    public float m_OnTimeRandomization;
    public float m_OffTime;
    public float m_OffTimeRandomization;
    public bool m_ActivateOnStart;
    public float m_ActivationDelay;
    public float m_WarmUpTime;
    public float m_CoolDownTime;
    public UnityEvent m_OnActivated;
    public UnityEvent m_OnDeactivated;

    private float m_TimeRemaining;
    private bool m_IsOn;
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (m_ActivateOnStart)
        {
            Activate();
        } else
        {
            Deactivate();
        }
    }

    private void Update()
    {
        m_TimeRemaining -= Time.deltaTime;
        if (m_TimeRemaining <= 0)
        {
            if (m_IsOn && m_OffTime > 0)
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
        m_TimeRemaining = m_WarmUpTime + m_OnTime + Random.Range(0, m_OnTimeRandomization);
        m_Animator.SetBool("On", true);
        m_Animator.SetFloat("TransitionSpeed", 1 / m_WarmUpTime);
    }

    private void TurnOff()
    {
        m_IsOn = false;
        m_TimeRemaining = m_CoolDownTime + m_OffTime + Random.Range(0, m_OffTimeRandomization);
        m_Animator.SetBool("On", false);
        m_Animator.SetFloat("TransitionSpeed", 1 / m_CoolDownTime);
    }

    public void Activate()
    {
        if (m_ActivationDelay > 0)
        {
            TurnOff();
            m_TimeRemaining = m_ActivationDelay;
        } else
        {
            TurnOn();
        }
        m_OnActivated.Invoke();
        enabled = true;
    }

    public void Deactivate()
    {
        TurnOff();
        m_OnDeactivated.Invoke();
        enabled = false;
    }
}
