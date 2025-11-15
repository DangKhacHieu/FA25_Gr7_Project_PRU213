using UnityEngine;

public class Path_Map1 : MonoBehaviour
{
    [Header("Danh sách waypoint theo thứ tự di chuyển")]
    [SerializeField] private Transform[] waypoints;

    public int WaypointCount => waypoints.Length;

    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Length)
        {
            Debug.LogWarning($" Index {index} vượt quá số lượng waypoint ({waypoints.Length}) trong {name}.");
            return null; // Trả về null khi index không hợp lệ
        }

        // KIỂM TRA QUAN TRỌNG: Kiểm tra xem Waypoint có bị trống (null) trong Editor không
        if (waypoints[index] == null)
        {
            Debug.LogError($"Waypoint index {index} trong {name} bị bỏ trống (null reference).");
            return null; // Trả về null khi Waypoint bị trống trong Editor
        }

        return waypoints[index];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
#endif
}
