using UnityEngine;

public class StraightWalkDetector : MonoBehaviour
{
    [Header("필수 연결")]
    public Transform userHMD;   // 플레이어 카메라
    public Transform endPoint;  // 횡단보도 건너편 도착점 (중앙에 배치 필수!)

    // 횡단보도 객체는 이제 방향 계산용이 아니라, 시작 위치 참조용으로만 씁니다.
    // (만약 시작점이 유동적이라면 이 변수는 사실상 없어도 됩니다)
    public Transform crosswalkStartPoint;

    [Header("감도 설정")]
    public float warningThreshold = 0.5f; // 이탈 허용 범위 (미터)

    [Header("옵션")]
    // 체크하면: 플레이어가 '출발한 위치'를 기준으로 직선을 그립니다. (평행 이동)
    // 체크 해제하면: 무조건 '횡단보도 중앙'을 기준으로 직선을 그립니다.
    public bool useRelativePath = true;

    // --- 외부 정보 ---
    public bool IsDeviated { get; private set; }
    public float CurrentDeviation { get; private set; }
    public string DirectionFeedback { get; private set; }

    // 내부 계산용 변수
    private Vector3 pathStartPos;   // 경로의 시작점
    private Vector3 pathDirection;  // 경로의 방향 (단위 벡터)
    private bool isMonitoring = false;
    private float lastLogTime = 0f;

    public void StartSensor()
    {
        // 1. 방향 벡터 계산 (시작점 -> 도착점)
        // 횡단보도 에셋의 회전값이 이상해도, 도착점만 잘 찍으면 정확한 직선이 나옵니다.
        // 높이(y) 차이는 무시하고 평면상 방향만 구합니다.
        Vector3 startPos = (crosswalkStartPoint != null) ? crosswalkStartPoint.position : userHMD.position;
        Vector3 endPos = endPoint.position;

        Vector3 direction = endPos - startPos;
        direction.y = 0; // 높이 무시
        pathDirection = direction.normalized; // 방향 벡터 확정

        // 2. 경로의 기준점(시작점) 설정
        if (useRelativePath)
        {
            // [사용자 맞춤] 플레이어가 서 있는 바로 그 위치에서 시작하는 직선 생성
            pathStartPos = userHMD.position;
            pathStartPos.y = 0;
        }
        else
        {
            // [절대 중앙] 무조건 횡단보도 객체의 위치를 기준으로 직선 생성
            pathStartPos = startPos;
            pathStartPos.y = 0;
        }

        isMonitoring = true;
        IsDeviated = false;
        Debug.Log($"📡 센서 보정 완료. 방향: {pathDirection}, 기준점: {pathStartPos}");
    }

    public void StopSensor()
    {
        isMonitoring = false;
        IsDeviated = false;
    }

    void Update()
    {
        if (!isMonitoring) return;

        // 1. 플레이어 현재 위치 (높이 무시)
        Vector3 currentUserPos = userHMD.position;
        currentUserPos.y = 0;

        // 2. 벡터 연산으로 이탈 거리 계산 (Cross Product 활용)
        // 경로 방향 벡터와 플레이어 위치 벡터의 외적(Cross Product)의 Y값은
        // 직선 거리를 의미하며, 부호(+/-)로 왼쪽/오른쪽을 알 수 있습니다.

        Vector3 vectorToUser = currentUserPos - pathStartPos;

        // 외적 계산: (직선 방향) x (내 위치 벡터)
        // 결과값의 Y가 양수면 오른쪽, 음수면 왼쪽입니다. (왼손 좌표계 기준)
        float deviationCheck = Vector3.Cross(pathDirection, vectorToUser).y;

        CurrentDeviation = deviationCheck;

        // 3. 판정 로직
        if (Mathf.Abs(CurrentDeviation) > warningThreshold)
        {
            IsDeviated = true;

            if (CurrentDeviation > 0)
            {
                DirectionFeedback = "Left"; // 오른쪽(+) 이탈 -> 왼쪽 지시
                LogFeedback("◀️ 왼쪽으로 이동하세요", CurrentDeviation);
            }
            else
            {
                DirectionFeedback = "Right"; // 왼쪽(-) 이탈 -> 오른쪽 지시
                LogFeedback("▶️ 오른쪽으로 이동하세요", CurrentDeviation);
            }
        }
        else
        {
            IsDeviated = false;
            DirectionFeedback = "Straight";
        }
    }

    void LogFeedback(string msg, float distance)
    {
        if (Time.time - lastLogTime > 1.0f)
        {
            Debug.Log($"⚠️ {Mathf.Abs(distance):F2}m 이탈! ({msg})");
            lastLogTime = Time.time;
        }
    }

    // [디버깅용] 씬 화면에 가상의 직선 그려주기 (이게 진짜 꿀기능!)
    void OnDrawGizmos()
    {
        if (isMonitoring)
        {
            Gizmos.color = Color.green;
            // 기준선 그리기
            Gizmos.DrawLine(pathStartPos, pathStartPos + pathDirection * 20f);

            // 플레이어 위치 표시
            Gizmos.color = Color.red;
            Vector3 userPosFlat = userHMD.position; userPosFlat.y = 0;
            Gizmos.DrawSphere(userPosFlat, 0.1f);
        }
    }
}