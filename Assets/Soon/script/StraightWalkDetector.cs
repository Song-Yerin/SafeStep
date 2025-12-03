using UnityEngine;

public class StraightWalkDetector : MonoBehaviour
{
    [Header("필수 연결")]
    public Transform crosswalk; // 횡단보도 오브젝트 (기준)
    public Transform userHMD;   // 플레이어 (Camera)

    [Header("감도 설정")]
    public float warningThreshold = 0.5f; // 이 정도 벗어나면 경고 (미터)

    // --- 외부(Manager)에서 가져다 쓸 정보들 ---
    public bool IsDeviated { get; private set; } // 이탈 여부 (True면 경고)
    public float CurrentDeviation { get; private set; } // 현재 이탈 거리 (+:오른쪽, -:왼쪽)
    public string DirectionFeedback { get; private set; } // "왼쪽으로 가세요" 같은 안내 텍스트

    private float lockedX; // 출발 시 기준 X좌표
    private bool isMonitoring = false;

    // 감시 시작 (Manager가 호출)
    public void StartSensor()
    {
        // 현재 위치를 기준으로 0점 조절
        Vector3 localPos = crosswalk.InverseTransformPoint(userHMD.position);
        lockedX = localPos.x;

        isMonitoring = true;
        IsDeviated = false;
        Debug.Log("📡 센서 작동 시작");
    }

    // 감시 종료 (Manager가 호출)
    public void StopSensor()
    {
        isMonitoring = false;
        IsDeviated = false;
    }

    void Update()
    {
        if (!isMonitoring) return;

        // 1. 현재 위치 계산 (횡단보도 기준 로컬 좌표)
        Vector3 currentLocalPos = crosswalk.InverseTransformPoint(userHMD.position);

        // 2. 이탈 거리 계산 (현재 X - 기준 X)
        CurrentDeviation = currentLocalPos.x - lockedX;

        // 3. 판정 로직
        if (Mathf.Abs(CurrentDeviation) > warningThreshold)
        {
            IsDeviated = true;

            // 오른쪽(+)으로 갔으면 -> "왼쪽으로 가세요"
            if (CurrentDeviation > 0) DirectionFeedback = "Left";
            // 왼쪽(-)으로 갔으면 -> "오른쪽으로 가세요"
            else DirectionFeedback = "Right";
        }
        else
        {
            IsDeviated = false;
            DirectionFeedback = "Straight";
        }
    }
}