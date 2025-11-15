using System.Collections.Generic;
using UnityEngine;

public class PathManager_map_4 : MonoBehaviour
{
    // 1. Kéo TẤT CẢ các path (Path1, Path2, Path3...) trong Scene vào đây
    [SerializeField] private Path_map_4[] allPaths;

    // Dictionary để tra cứu nhanh bằng tên
    private Dictionary<string, Path_map_4> pathDictionary;

    // Singleton pattern để Spawner dễ dàng truy cập
    public static PathManager_map_4 Instance { get; private set; }

    private void Awake()
    {
        // === Setup Singleton ===
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // === Setup Dictionary ===
        // Tạo một "danh bạ" để tra cứu tên path
        pathDictionary = new Dictionary<string, Path_map_4>();
        foreach (Path_map_4 path in allPaths)
        {
            if (path == null)
            {
                Debug.LogError("[PathManager] ❌ Một Path trong 'allPaths' bị null!");
                continue;
            }

            // Dùng tên của GameObject (ví dụ: "Path1") làm chìa khóa
            string pathGoName = path.gameObject.name;
            if (pathDictionary.ContainsKey(pathGoName))
            {
                Debug.LogWarning($"[PathManager] ⚠️ Tên path bị trùng: '{pathGoName}'.");
            }
            pathDictionary[pathGoName] = path;
        }
    }

    /// <summary>
    /// Lấy một Path object bằng tên (ví dụ: "Path1")
    /// </summary>
    public Path_map_4 GetPathByName(string pathName)
    {
        if (string.IsNullOrEmpty(pathName))
        {
            Debug.LogError("[PathManager] ❌ Tên path rỗng!");
            return null;
        }

        // Tra cứu path trong "danh bạ"
        if (pathDictionary.TryGetValue(pathName, out Path_map_4 path))
        {
            return path;
        }

        Debug.LogError($"[PathManager] ❌ Không tìm thấy path nào tên là: '{pathName}'");
        return null;
    }
}