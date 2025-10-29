using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 5;

    private List<GameObject> _pool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Create pool

        _pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++) 
        {
            CreateNewObject();
        }

    }

    private GameObject CreateNewObject()
    {
        GameObject obj =  Instantiate(prefab);
        obj.SetActive(false);
        _pool.Add(obj);
        return obj;
    }

    public GameObject GetPoolObject()
    {
        foreach (GameObject obj in _pool)
        {
            if (!obj.activeSelf)
            {
                return obj;
            }
        }
        return CreateNewObject();
    }
}
