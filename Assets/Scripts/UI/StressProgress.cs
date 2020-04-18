using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressProgress : MonoBehaviour
{
    [SerializeField]
    private Slider m_StressProgress;

    public void SetProgress(float value)
    {
        value = Mathf.Clamp01(value);
        m_StressProgress.value = value;
    }
}
