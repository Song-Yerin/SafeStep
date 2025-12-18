using UnityEngine;
using System.Collections;
public class CarSpawner : MonoBehaviour
{
    [Header("차량 프리팹들")]
    public GameObject[] carPrefabs;

    [Header("스폰 설정")]
    public SpawnPoint[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxCars = 20;

    private int currentCarCount = 0;
    private int currentSpawnIndex = 0;  // 🔴 추가

    public static CarSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (carPrefabs.Length == 0)
        {
            Debug.LogError("차량 프리팹이 없습니다!");
            return;
        }
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("스폰 포인트가 없습니다!");
            return;
        }
        StartCoroutine(SpawnCars());
    }

    IEnumerator SpawnCars()
    {
        while (true)
        {
            if (currentCarCount < maxCars)
            {
                GameObject randomCar = carPrefabs[Random.Range(0, carPrefabs.Length)];
                SpawnPoint spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // 🔴 0번 웨이포인트 위치에서 스폰
                Vector3 spawnPosition = spawn.routeWaypoints.Length > 0
                    ? spawn.routeWaypoints[0].position
                    : spawn.transform.position;

                Quaternion spawnRotation = spawn.routeWaypoints.Length > 1
                    ? Quaternion.LookRotation(spawn.routeWaypoints[1].position - spawn.routeWaypoints[0].position)
                    : spawn.transform.rotation;

                GameObject car = Instantiate(randomCar, spawnPosition, spawnRotation);

                CarAI carAI = car.GetComponent<CarAI>();
                if (carAI != null && spawn.routeWaypoints.Length > 0)
                {
                    carAI.waypoints = spawn.routeWaypoints;
                }
                else
                {
                    Debug.LogWarning("CarAI 또는 웨이포인트가 없습니다!");
                }

                currentCarCount++;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public void OnCarDestroyed()
    {
        currentCarCount--;
    }
}