using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPlacement : MonoBehaviour
{
    public static void SpawnAsset(Vector3 pos, /*Vector2 mid, float assetOffset, Vector2 assetMultiplier,*/ GameObject prefab, Transform prefabParent)
    {
        GameObject obj = Instantiate(prefab, pos, Quaternion.Euler(0,Random.Range(0, 360f), 0));
        obj.name = "newObj";
        obj.transform.SetParent(prefabParent.transform);
    }
}
