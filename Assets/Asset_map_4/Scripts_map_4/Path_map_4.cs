#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class Path_map_4 : MonoBehaviour
{
    public GameObject[] Waypoints;

    public Vector3 GetPosition(int index)
    {
        return Waypoints[index].transform.position;
    }
    private void Awake()
    {
        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (Waypoints[i] == null)
            {
                Debug.LogError($"[Path_map_4] ❌ Waypoint tại index {i} của path '{gameObject.name}' bị null!", this);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (Waypoints.Length > 0)
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                #if UNITY_EDITOR
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(Waypoints[i].transform.position + Vector3.up * 0.7f, Waypoints[i].name, style);
                #endif

                if (i < Waypoints.Length - 1)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[i + 1].transform.position);
                }
            }
        }
    }
}
