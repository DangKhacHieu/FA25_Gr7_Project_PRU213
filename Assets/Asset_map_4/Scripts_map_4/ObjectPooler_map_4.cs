using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler_map_4 : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 5;
    private List<GameObject> _pool;

    void Awake()
    {
        // Tạo pool sớm (tránh event gọi trước Start)
        InitializePool();
    }

    private void InitializePool()
    {
        if (_pool != null) return;

        _pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
            CreateNewObject();
    }

    private GameObject CreateNewObject()
    {
        if (prefab == null)
        {
            Debug.LogError("[ObjectPooler] ❌ Prefab chưa được gán!");
            return null;
        }

        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false); 
        _pool.Add(obj);
        return obj;
    }

    public GameObject GetPooledObject()
    {
        if (_pool == null || _pool.Count == 0)
            InitializePool();

        // Lọc bỏ object null hoặc bị Destroy
        for (int i = _pool.Count - 1; i >= 0; i--)
        {
            if (_pool[i] == null)
                _pool.RemoveAt(i);
        }

        // Tìm object đang tắt để dùng lại
        foreach (GameObject obj in _pool)
        {
            if (obj != null && !obj.activeSelf)
                return obj;
        }

        // Nếu hết → tạo mới
        return CreateNewObject();
    }
}
