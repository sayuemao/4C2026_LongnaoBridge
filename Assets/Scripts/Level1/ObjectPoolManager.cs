using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        ClearAllPools();
    }

    public void ClearAllPools()
    {
        foreach (var pool in objectPools)
        {
            if (pool.Value != null)
            {
                while (pool.Value.Count > 0)
                {
                    GameObject obj = pool.Value.Dequeue();
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }
        }
        objectPools.Clear();
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        Queue<GameObject> pool = objectPools[prefab];

        while (pool.Count > 0 && pool.Peek() == null)
        {
            pool.Dequeue();
        }

        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();

            if (obj == null)
            {
                return CreateNewObject(prefab);
            }

            obj.SetActive(true);
            return obj;
        }
        else
        {
            return CreateNewObject(prefab);
        }
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null || obj.Equals(null))
        {
            return;
        }

        obj.SetActive(false);

        GameObject prefab = FindPrefabForObject(obj);

        if (prefab != null && objectPools.ContainsKey(prefab))
        {
            objectPools[prefab].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    private GameObject FindPrefabForObject(GameObject obj)
    {
        if (obj == null) return null;

        string objName = obj.name;

        foreach (var pair in objectPools)
        {
            if (pair.Key != null)
            {
                string prefabName = pair.Key.name;
                if (objName.StartsWith(prefabName) ||
                    objName.StartsWith(prefabName + "(clone)") ||
                    objName.Replace("(clone)", "").Trim() == prefabName)
                {
                    return pair.Key;
                }
            }
        }

        return null;
    }
}