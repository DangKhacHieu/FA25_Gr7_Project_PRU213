using System.Collections;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    private int wavesCompleted = 0;
    public enum PathType { A, B }

    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnDelay = 1f;
        public PathType path = PathType.A;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public EnemyGroup[] enemyGroups;
        public int reward;
    }

    [Header("Cấu hình Wave")]
    public Wave[] waves;
    private int currentWaveIndex = 0;

    [Header("Điểm sinh quái")]
    public Transform spawnPointA;
    public Transform spawnPointB;

    [Header("Thời gian giữa các wave")]
    public float timeBetweenWaves = 5f;

    private bool isSpawning = false;
    private bool allWavesSpawned = false;
    private int aliveEnemies = 0;
    private bool winTriggered = false;
    private bool gameOverTriggered = false; // 👈 Cờ Game Over thật sự

    void Update()
    {
        if (isSpawning || gameOverTriggered) return;

        if (currentWaveIndex < waves.Length)
        {
            isSpawning = true;
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
        else if (!allWavesSpawned)
        {
            Debug.Log("✅ Đã spawn hết tất cả wave!");
            allWavesSpawned = true;
        }

        // ✅ Chỉ hiển thị WIN nếu chưa thua
        if (!gameOverTriggered && !winTriggered && allWavesSpawned && aliveEnemies == 0)
        {
            GateHealth gate = FindFirstObjectByType<GateHealth>();
            if (gate != null && gate.IsAlive())
            {
                Debug.Log("🏆 Không còn quái và cổng sống → WIN!");
                LevelCompleteManager complete = FindFirstObjectByType<LevelCompleteManager>();
                if (complete != null)
                    complete.ShowWinUI();

                winTriggered = true;
                enabled = false;
            }
        }
    }

    public void TriggerGameOverFlag()
    {
        Debug.Log("❌ Game Over flag đã bật — không cho hiện Win nữa.");
        typeof(EnemyWaveSpawner)
            .GetField("gameOverTriggered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(this, true);
    }


    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;
        Debug.Log($"🌊 Bắt đầu {wave.waveName}");

        if (WaveUI.Instance != null)
            WaveUI.Instance.UpdateWaveText(currentWaveIndex + 1);

        foreach (EnemyGroup group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyPrefab, group.path);
                yield return new WaitForSeconds(group.spawnDelay);
            }
        }

        Debug.Log($"✅ Hoàn thành sinh wave {wave.waveName}");

        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = false;

        if (wave.reward > 0 && GoldManager.Instance != null)
            GoldManager.Instance.AddGold(wave.reward);

        currentWaveIndex++;
        if (currentWaveIndex >= waves.Length)
            allWavesSpawned = true;
    }

    void SpawnEnemy(GameObject enemyPrefab, PathType pathType)
    {
        Transform spawnPos = (pathType == PathType.A) ? spawnPointA : spawnPointB;
        GameObject e = Instantiate(enemyPrefab, spawnPos.position, spawnPos.rotation);

        EnemyHealth health = e.GetComponent<EnemyHealth>();
        if (health != null)
            health.OnEnemyDie += HandleEnemyDeath;

        EnemyMovement move = e.GetComponent<EnemyMovement>();
        if (move != null)
        {
            move.pathType = (EnemyMovement.PathType)pathType;
            move.spawner = this;
        }

        aliveEnemies++;
    }

    void HandleEnemyDeath()
    {
        aliveEnemies--;
        if (aliveEnemies < 0) aliveEnemies = 0;
    }

    public void NotifyEnemyRemoved()
    {
        aliveEnemies--;
        if (aliveEnemies < 0) aliveEnemies = 0;
        Debug.Log($"Enemy removed, aliveEnemies = {aliveEnemies}");
    }
}
