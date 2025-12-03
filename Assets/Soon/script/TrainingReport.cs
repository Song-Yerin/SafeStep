using UnityEngine;

public class TrainingReport : MonoBehaviour
{
    [Header("스크립트 연결")]
    public StraightWalkDetector detector; // 방금 만든 센서 스크립트 연결

    [Header("오브젝트 연결")]
    public Transform userHMD;  // 플레이어 카메라
    public Transform endPoint; // 도착 지점 (Destination)

    [Header("훈련 설정")]
    public float arrivalDistance = 1.0f; // 도착 인정 거리

    // 데이터 기록용
    private float totalTime = 0f;
    private float safeTime = 0f;
    private int hitCount = 0;
    private bool isTraining = false;

    void Update()
    {
        // 테스트용: 스페이스바 누르면 훈련 시작 (나중에 VR 버튼으로 교체 가능)
        if (Input.GetKeyDown(KeyCode.Space) && !isTraining)
        {
            BeginTraining();
        }

        if (!isTraining) return;

        // 1. 시간 측정
        totalTime += Time.deltaTime;

        // 2. 센서 상태 확인 및 점수 계산
        if (detector.IsDeviated)
        {
            // 이탈 중일 때 피드백 (콘솔에만 표시, 나중에 소리로 연결)
            // Debug.Log($"⚠️ 경고! {detector.DirectionFeedback} 쪽으로 이동하세요!");
        }
        else
        {
            // 잘 가고 있으면 점수 시간 추가
            safeTime += Time.deltaTime;
        }

        // 3. 도착 체크
        CheckArrival();
    }

    public void BeginTraining()
    {
        isTraining = true;
        totalTime = 0f;
        safeTime = 0f;
        hitCount = 0;

        // 센서 켜기
        detector.StartSensor();
        Debug.Log("🚀 훈련 프로세스 시작! (목표 지점으로 이동하세요)");
    }

    void CheckArrival()
    {
        // 높이(Y) 무시하고 수평 거리만 계산
        float dist = Vector3.Distance(
            new Vector3(userHMD.position.x, 0, userHMD.position.z),
            new Vector3(endPoint.position.x, 0, endPoint.position.z)
        );

        if (dist <= arrivalDistance)
        {
            EndTraining();
        }
    }

    void EndTraining()
    {
        isTraining = false;
        detector.StopSensor(); // 센서 끄기

        // 최종 리포트 계산
        float score = 0f;
        if (totalTime > 0) score = (safeTime / totalTime) * 100f;

        Debug.Log("🏁 훈련 종료! 목적지 도착.");
        Debug.Log("============== 📋 훈련 리포트 ==============");
        Debug.Log($"⏱️ 총 소요 시간: {totalTime:F2}초");
        Debug.Log($"✅ 직선 보행 유지율: {score:F1}%");
        Debug.Log($"💥 장애물 충돌: {hitCount}회");
        Debug.Log($"💡 피드백: {(score > 80 ? "아주 훌륭합니다!" : "조금 더 직선 유지 연습이 필요합니다.")}");
        Debug.Log("==========================================");
    }

    // 장애물 충돌 시 외부에서 호출할 함수
    public void AddHitCount()
    {
        if (isTraining) hitCount++;
    }
}