using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Breakable))]
public class StressSource : MonoBehaviour
{
    public float m_StressPerSecond;

    private void Update()
    {
        GameManager.Instance.m_Stress += m_StressPerSecond * Time.deltaTime;
    }
}
