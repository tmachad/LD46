using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unscalable : MonoBehaviour
{
    private void Update()
    {
        if (transform.parent)
        {
            Vector3 totalScale = transform.parent.lossyScale;
            Vector3 offset = new Vector3(
                1 / totalScale.x,
                1 / totalScale.y,
                1 / totalScale.z
            );
            transform.localScale = offset;
        }
    }
}
