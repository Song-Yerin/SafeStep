using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    [Header("차량 프리팹들")]
    public GameObject[] carPrefabs;

    [Header("스폰 설정")]
    public SpawnPoint[] spawnPoints;  // Transform[] 대신 SpawnPoint[]
    public float spawnInterval = 5f;
    public int maxCars = 20;

    private int currentCarCount = 0;

    void Start()
    {
        // 배열 체크
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
                // 랜덤 차량 선택
                GameObject randomCar = carPrefabs[Random.Range(0, carPrefabs.Length)];

                // 랜덤 스폰 위치
                SpawnPoint spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // 차량 생성
                GameObject car = Instantiate(randomCar, spawn.transform.position, spawn.transform.rotation);

                // 웨이포인트 할당!
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
}