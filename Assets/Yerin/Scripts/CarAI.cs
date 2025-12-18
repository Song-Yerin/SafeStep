using UnityEngine;

public class CarAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 10f;
    public float rotationSpeed = 5f;
    public float waypointThreshold = 2f;

    [Header("차량 감지")]
    public float detectionDistance = 5f;  // 🔴 추가
    public LayerMask carLayer;  // 🔴 추가

    private int currentWaypoint = 0;
    private bool canMove = true;
    private bool blockedByCar = false;  // 🔴 추가
    private AudioSource engineSound;

    void Start()
    {
        engineSound = GetComponent<AudioSource>();

        if (engineSound != null)
        {
            engineSound.spatialBlend = 1f;
            engineSound.minDistance = 5f;
            engineSound.maxDistance = 50f;
            engineSound.loop = true;
            engineSound.Play();
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 🔴 앞차 감지
        CheckForCarAhead();

        UpdateEngineSound();

        // 🔴 신호등 또는 앞차 때문에 못가면 정지
        if (!canMove || blockedByCar) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 targetPosition = target.position;

        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < waypointThreshold)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                if (CarSpawner.Instance != null)
                {
                    CarSpawner.Instance.OnCarDestroyed();
                }
                Destroy(gameObject);
                return;
            }
        }
    }

    // 🔴 앞차 감지
    void CheckForCarAhead()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayStart, transform.forward, out hit, detectionDistance, carLayer))
        {
            // 앞에 차가 있으면 정지
            CarAI otherCar = hit.collider.GetComponent<CarAI>();
            if (otherCar != null && otherCar != this)
            {
                blockedByCar = true;
                return;
            }
        }

        blockedByCar = false;
    }

    void UpdateEngineSound()
    {
        if (engineSound == null) return;

        // 🔴 신호등이나 앞차 때문에 못가면 소리 줄임
        bool shouldMove = canMove && !blockedByCar;

        if (shouldMove)
        {
            if (!engineSound.isPlaying)
                engineSound.Play();

            engineSound.volume = Mathf.Lerp(engineSound.volume, 1f, Time.deltaTime * 5f);
        }
        else
        {
            engineSound.volume = Mathf.Lerp(engineSound.volume, 0f, Time.deltaTime * 5f);

            if (engineSound.volume < 0.01f)
            {
                engineSound.Stop();
            }
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    // 🔴 디버그용 (Scene 뷰에서 감지 범위 보기)
    void OnDrawGizmos()
    {
        Gizmos.color = blockedByCar ? Color.red : Color.green;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawRay(rayStart, transform.forward * detectionDistance);
    }
}


public enum CarType
{
    Sedan,
    SUV,
    Truck,
    SportsCar,
    Van
}