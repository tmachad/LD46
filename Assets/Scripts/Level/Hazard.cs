using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hazard : MonoBehaviour
{
    public LayerMask m_PlayerLayer;

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
}
