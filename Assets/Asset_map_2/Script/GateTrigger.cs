using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GateTrigger : MonoBehaviour
{
    public GateHealth gate;         // 👈 đổi từ CastleHealth sang GateHealth
    public string enemyTag = "Enemy";
    public int damagePerEnemy = 1;

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        EnemyMovement move = other.GetComponent<EnemyMovement>();
        if (move != null && move.isBoss)
        {
            Debug.Log("🚫 GateTrigger: bỏ qua boss vì xử lý riêng trong EnemyMovement!");
            return;
        }

        if (!other.CompareTag(enemyTag)) return;

        // ✅ Gọi GateHealth để trừ máu và hiện UI khi hết máu
        if (gate != null)
        {
            gate.TakeDamage(damagePerEnemy);
            Debug.Log($"⚔️ Enemy {other.name} hit the gate! -{damagePerEnemy} HP");
        }

        // ✅ Báo cho spawner biết quái đã vào thành (giảm aliveEnemies)
        EnemyWaveSpawner spawner = FindFirstObjectByType<EnemyWaveSpawner>();
        if (spawner != null)
        {
            spawner.NotifyEnemyRemoved();
            Debug.Log("📉 Enemy vào thành → NotifyEnemyRemoved()");
        }

        Destroy(other.gameObject);
    }

}
