using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WarningLight : MonoBehaviour
{
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void TurnOn()
    {
        m_Animator.SetInteger("Mode", 1);
    }

    public void TurnOff()
    {
        m_Animator.SetInteger("Mode", 0);
    }

    public void Blink()
    {
        m_Animator.SetInteger("Mode", 2);
    }
}
