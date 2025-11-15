using UnityEngine;

public class WaypointsA : MonoBehaviour
{
    [HideInInspector] public Transform[] points;

    void Awake()
    {
        points = new Transform[transform.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.GetChild(i);
        }
    }
}
