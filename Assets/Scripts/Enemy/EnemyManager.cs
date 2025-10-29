using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private readonly List<Enemy> activeEnemies = new List<Enemy>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public List<Enemy> GetActiveEnemies()
    {
        return activeEnemies;
    }
}
