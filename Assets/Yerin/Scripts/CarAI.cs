using UnityEngine;

public class CarAI : MonoBehaviour
{
    public Transform[] waypoints;  // public으로 외부에서 할당 가능
    public float speed = 10f;
    public float stoppingDistance = 2f;

    private int currentWaypoint = 0;
    private bool canMove = true;

    void Update()
    {
        // 웨이포인트 없으면 작동 안 함
        if (waypoints == null || waypoints.Length == 0) return;
        if (!canMove) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 direction = (target.position - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(target);

        if (Vector3.Distance(transform.position, target.position) < stoppingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    public void SetCanMove(bool value) => canMove = value;
}

public enum CarType
{
    Sedan,
    SUV,
    Truck,
    SportsCar,
    Van
}