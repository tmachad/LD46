﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public GameObject m_FollowTarget;
    public Bounds m_Bounds;

    [Header("Camera Shake")]
    public float m_ShakeDecayRate;
    public float m_ShakeStrength;

    private Camera m_Camera;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Vector3 cameraPos = m_Camera.transform.position;
        Vector3 targetPos = m_FollowTarget.transform.position;
        Vector2 cameraHalfSize = new Vector2(m_Camera.orthographicSize * m_Camera.aspect, m_Camera.orthographicSize);
        Rect camBounds = new Rect(
            m_Bounds.min.x,
            m_Bounds.min.y,
            m_Bounds.size.x,
            m_Bounds.size.y
        );

        cameraPos.x = targetPos.x;
        cameraPos.y = targetPos.y;

        if (cameraHalfSize.y >= camBounds.height / 2)
        {
            // Camera is too tall to fit in bounds, center vertically
            cameraPos.y = camBounds.center.y;
        }
        else if (cameraPos.y + cameraHalfSize.y > camBounds.max.y)
        {
            // Camera will be outside top of bounds, snap to top of bounds instead
            cameraPos.y = camBounds.max.y - cameraHalfSize.y;
        }
        else if (cameraPos.y - cameraHalfSize.y < camBounds.min.y)
        {
            // Camera will be outside bottom of bounds, snap to bottom of bounds insteads
            cameraPos.y = camBounds.min.y + cameraHalfSize.y;
        }

        if (cameraHalfSize.x >= camBounds.width / 2)
        {
            // Camera is too wide to fit in bounds, center horizontally
            cameraPos.x = camBounds.center.x;
        }
        else if (cameraPos.x + cameraHalfSize.x > camBounds.max.x)
        {
            // Camera will be outside right edge of bounds, snap to right side of bounds instead
            cameraPos.x = camBounds.max.x - cameraHalfSize.x;
        }
        else if (cameraPos.x - cameraHalfSize.x < camBounds.min.x)
        {
            // Camera will be outside left edge of bounds, snap to left edge of bounds instead
            cameraPos.x = camBounds.min.x + cameraHalfSize.x;
        }

        if (m_ShakeStrength > 0)
        {
            cameraPos = cameraPos + (Vector3)(Random.insideUnitCircle.normalized * m_ShakeStrength);
            m_ShakeStrength -= m_ShakeDecayRate * Time.deltaTime;
        }

        m_Camera.transform.position = cameraPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_Bounds.center, m_Bounds.size);
    }
}
