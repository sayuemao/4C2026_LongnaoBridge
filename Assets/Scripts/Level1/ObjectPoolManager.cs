using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance{get;private set;}

    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        Queue<GameObject> pool = objectPools[prefab];
        
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(true);
            return newObj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        
        // 获取对象所属的预制体
        GameObject prefab = null;
        foreach (var pair in objectPools)
        {
            if (pair.Key != null && obj.name.StartsWith(pair.Key.name))
            {
                prefab = pair.Key;
                break;
            }
        }
        
        if (prefab != null)
        {
            objectPools[prefab].Enqueue(obj);
        }
    }
}
