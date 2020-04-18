using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PixelPerfectCamera : MonoBehaviour
{

    public int m_PixelsPerUnit;
    public int m_Doubling = 0;

    private Camera m_Camera;
    private int m_PrevHeight;
    private int m_PrevPPU;
    private int m_PrevDoubling;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    private void OnPreRender()
    {
        if (m_Camera.pixelHeight != m_PrevHeight || m_PixelsPerUnit != m_PrevPPU || m_Doubling != m_PrevDoubling)
        {
            m_PrevHeight = m_Camera.pixelHeight;
            m_PrevPPU = m_PixelsPerUnit;
            m_PrevDoubling = m_Doubling;
            m_Camera.orthographicSize = m_Camera.pixelHeight / (m_PixelsPerUnit * 2f * Mathf.Pow(2f, m_Doubling));
        }
    }
}