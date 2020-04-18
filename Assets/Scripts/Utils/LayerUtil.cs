using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUtil
{
    public static bool GameObjectInLayerMask(GameObject obj, LayerMask layer)
    {
        return layer == (layer | 1 << obj.layer);
    }
}
