using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static event Action<int> OnLivesChanged;

    private int _lives = 3;

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HanldeEnemyReachesEnd;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HanldeEnemyReachesEnd;
    }

    private void Start()
    {
        OnLivesChanged?.Invoke(_lives);
    }
    private void HanldeEnemyReachesEnd(EnemyData data)
    {
        _lives = Mathf.Max(0,_lives - data.damage);
        OnLivesChanged?.Invoke(_lives);
    }
}


