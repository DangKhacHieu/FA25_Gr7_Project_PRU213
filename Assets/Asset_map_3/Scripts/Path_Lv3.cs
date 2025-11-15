using Unity.Jobs;
using UnityEditor;
using UnityEngine;

public class Path_Lv3 : MonoBehaviour
{
    public GameObject[] Waypoints;

    public Vector3 GetPosition(int index)
    {
        return Waypoints[index].transform.position;
    }

    public int GetLength()
    {
        return Waypoints.Length;
    }

    private void OnDrawGizmos()
    {
       /* if(Waypoints.Length > 0)
        {
            for(int i = 0; i < Waypoints.Length; i++)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(Waypoints[i].transform.position + Vector3.up * 0.7f,
                    Waypoints[i].name, style);

                if (i <  Waypoints.Length - 1)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[i + 1].
                        transform.position);
                }
                
            }
        }*/

        if (Waypoints == null || Waypoints.Length == 0) return;

        /*for (int i = 0; i < Waypoints.Length; i++)
        {
            // Hiển thị label
            GUIStyle style = new GUIStyle
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };
            Handles.Label(Waypoints[i].transform.position + Vector3.up * 0.7f, Waypoints[i].name, style);

            // Vẽ đường nối giữa các waypoint
            if (i < Waypoints.Length - 1)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[i + 1].transform.position);
            }
        }*/

        GUIStyle style = new GUIStyle
        {
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter
        };

        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (Waypoints[i] == null) continue;

            Handles.Label(Waypoints[i].transform.position + Vector3.up * 0.7f, Waypoints[i].name, style);

            if (i < Waypoints.Length - 1 && Waypoints[i + 1] != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[i + 1].transform.position);
            }
        }
    }

}
