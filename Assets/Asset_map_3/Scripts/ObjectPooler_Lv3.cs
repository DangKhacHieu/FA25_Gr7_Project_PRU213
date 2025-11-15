using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ObjectPooler_Lv3 : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 5;

    private List<GameObject> _pool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Create pool

        _pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++) 
        {
            CreateNewObject();
        }

    }

    /* private GameObject CreateNewObject()
     {
         GameObject obj =  Instantiate(prefab);
         obj.SetActive(false);
         _pool.Add(obj);
         return obj;
     }*/

    // --- ĐÂY LÀ HÀM ĐÃ SỬA LẠI CHO ĐÚNG ---
    private GameObject CreateNewObject()
    {
        // 1. Dùng biến 'prefab' của bạn
        GameObject obj = Instantiate(prefab);

        // --- THÊM DÒNG NÀY ---
        // Biến object mới thành con của pool (cho gọn gàng)
        obj.transform.SetParent(transform);
        // --- HẾT THÊM MỚI ---
                    
        // 2. Tắt nó đi (Đây là mấu chốt để sửa lỗi 'OnEnable')
        obj.SetActive(false);

        // 3. Thêm nó vào 'List' của bạn
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
