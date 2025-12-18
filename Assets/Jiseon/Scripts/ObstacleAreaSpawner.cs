using UnityEngine;
using System.Collections.Generic;

public class ObstacleAreaSpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    public BoxCollider spawnArea;

    [Header("Obstacle Prefab")]
    public GameObject obstaclePrefab;
    public int spawnCount = 3;

    [Header("Fallen Settings")]
    [Range(0f, 1f)]
    public float fallenProbability = 0.5f;
    public float fallenAngle = 90f;

    [Header("Spacing Settings")]
    public float minDistance = 1.2f;   // 최소 거리
    public int maxTries = 20;           // 위치 재시도 제한

    // 이미 생성된 위치들
    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        spawnedPositions.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos;
            bool found = TryGetSpacedPosition(out pos);

            if (!found)
            {
                Debug.LogWarning("장애물 위치를 충분히 떨어뜨릴 수 없어 생성 중단");
                break;
            }

            Quaternion rot = GetSpawnRotation();
            GameObject obj = Instantiate(obstaclePrefab, pos, rot);

            // 태그 자동 지정
            if (!obj.CompareTag("Obstacle"))
                obj.tag = "Obstacle";

            // 살짝 띄워서 박힘 방지
            obj.transform.position += Vector3.up * 0.02f;

            spawnedPositions.Add(pos);
        }
    }

    bool TryGetSpacedPosition(out Vector3 result)
    {
        for (int attempt = 0; attempt < maxTries; attempt++)
        {
            Vector3 candidate = GetRandomPositionInArea();

            bool tooClose = false;
            foreach (Vector3 existing in spawnedPositions)
            {
                if (Vector3.Distance(candidate, existing) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                result = candidate;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    Quaternion GetSpawnRotation()
    {
        if (Random.value < fallenProbability)
        {
            float xAngle = Random.value > 0.5f ? fallenAngle : -fallenAngle;
            float yAngle = Random.Range(0f, 360f);
            return Quaternion.Euler(xAngle, yAngle, 0f);
        }

        return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    Vector3 GetRandomPositionInArea()
    {
        Vector3 center = spawnArea.center + spawnArea.transform.position;
        Vector3 size = spawnArea.size;

        float x = Random.Range(-size.x / 2f, size.x / 2f);
        float z = Random.Range(-size.z / 2f, size.z / 2f);

        return center + new Vector3(x, 0f, z);
    }
}
