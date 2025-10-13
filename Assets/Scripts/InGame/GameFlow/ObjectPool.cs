using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // === Pool settings ===
    [SerializeField] private int defaultPoolSize;
    [SerializeField] private GameObject objectPrefab;
    private readonly List<GameObject> pooledObjects = new();

    void Awake()
    {
        ExpandPool(defaultPoolSize);
    }

    private void ExpandPool(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject newObj = Instantiate(objectPrefab);
            newObj.transform.SetParent(transform, false);
            newObj.SetActive(false);

            pooledObjects.Add(newObj);
        }
    }

    public GameObject GetObjectFromPool()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeSelf)
            {
                pooledObjects[i].SetActive(true);
                return pooledObjects[i];
            }
        }

        // If there are no objects available in the pool, it expands into one and returns the last one in the list
        ExpandPool(1);
        pooledObjects[^1].SetActive(true);
        return pooledObjects[^1];
    }
}