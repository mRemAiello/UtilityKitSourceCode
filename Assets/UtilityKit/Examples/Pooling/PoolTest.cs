using UnityEngine;
using UtilityKit;

public class PoolTest : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 spawnPosition;

    void Start()
    {
        GameObject spawnItem = Poolable.TryGetPoolable(prefab);
        spawnItem.transform.position = spawnPosition;
    }
}
