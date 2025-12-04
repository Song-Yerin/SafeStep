using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform[] routeWaypoints;  // 이 스폰 지점의 경로
    public Color gizmoColor = Color.green;

    // Scene 뷰에서 경로 시각화
    void OnDrawGizmos()
    {
        if (routeWaypoints == null || routeWaypoints.Length == 0) return;

        Gizmos.color = gizmoColor;
        for (int i = 0; i < routeWaypoints.Length; i++)
        {
            if (routeWaypoints[i] != null)
            {
                Gizmos.DrawSphere(routeWaypoints[i].position, 0.5f);

                if (i < routeWaypoints.Length - 1 && routeWaypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(routeWaypoints[i].position, routeWaypoints[i + 1].position);
                }
            }
        }
    }
}