using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public Vector2 m_Force;
    public LayerMask m_PlayerLayer;

    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerUtil.GameObjectInLayerMask(collision.gameObject, m_PlayerLayer))
        {
            m_AudioSource.Play();
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(m_Force, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector2 pos2d = (Vector2)transform.position;
        Vector2 arrowTip = pos2d + m_Force.normalized;

        Gizmos.DrawLine(pos2d, arrowTip);
    }
}
