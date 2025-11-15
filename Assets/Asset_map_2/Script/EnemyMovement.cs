using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [HideInInspector] public EnemyWaveSpawner spawner;
    public enum PathType { A, B }
    public PathType pathType = PathType.A;

    [Header("⚙️ Cấu hình di chuyển")]
    public float speed = 2f;

    [Header("💥 Sát thương khi vào thành")]
    public int damageToGate = 1;


    [Header("👑 Là Boss? (Tiny Golem, v.v.)")]
    public bool isBoss = false; // ✅ tick ô này cho boss trong prefab

    private Transform[] path;
    private Transform target;
    private int waypointIndex = 0;

    void Start()
    {
        if (pathType == PathType.A)
            path = FindFirstObjectByType<WaypointsA>().points;
        else
            path = FindFirstObjectByType<WaypointsB>().points;

        if (path == null || path.Length == 0)
        {
            Debug.LogWarning("⚠️ Không tìm thấy Waypoints cho " + pathType);
            return;
        }

        transform.position = path[0].position;
        waypointIndex = 0;
        target = path[1];
    }

    void Update()
    {
        if (path == null || target == null) return;

        Vector2 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            GetNextWaypoint();
        }
    }


    void GetNextWaypoint()
    {
        if (waypointIndex >= path.Length - 1)
        {
            GateHealth gate = FindFirstObjectByType<GateHealth>();
            if (gate != null)
            {
                Debug.Log($"⚔️ {name} hit the gate! -{damageToGate} HP");
                gate.TakeDamage(damageToGate);
            }

            if (spawner != null)
                spawner.NotifyEnemyRemoved();

            Destroy(gameObject);
            return;
        }

        waypointIndex++;
        target = path[waypointIndex];
    }

}
