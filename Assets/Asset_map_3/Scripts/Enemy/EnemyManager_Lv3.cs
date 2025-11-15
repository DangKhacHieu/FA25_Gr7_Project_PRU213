using System.Collections.Generic;
using UnityEngine;

public class EnemyManager_Lv3 : MonoBehaviour
{
    public static EnemyManager_Lv3 Instance;

    private readonly List<Enemy_Lv3> activeEnemies = new List<Enemy_Lv3>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(Enemy_Lv3 enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy_Lv3 enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    public List<Enemy_Lv3> GetActiveEnemies()
    {
        // Xóa quái null (an toàn)
        activeEnemies.RemoveAll(item => item == null);
        return activeEnemies;
    }
}
