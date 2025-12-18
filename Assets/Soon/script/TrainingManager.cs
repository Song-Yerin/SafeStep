using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingManager : MonoBehaviour
{
    // 3단계 상태 관리
    public enum TrainingState { Ready, Training, Finished }
    public TrainingState currentState = TrainingState.Ready;

    [Header("스크립트/오브젝트 연결")]
    public Camera mapCamera;
    public PathVisualizer pathVisualizer; // ⭐ 연결 필요!
    public StraightWalkDetector detector; // 센서
    public TextMeshProUGUI reportText;

    public Transform userHMD;             // 플레이어
    public Transform endPoint;            // 도착점
    public GameObject resultUIPanel;      // 결과창 UI

    [Header("오디오")]
    public AudioSource audioSource;
    public AudioClip guideVoiceClip;      // 종료 안내 음성

    [Header("설정")]
    public float arrivalDistance = 1.0f;  // 도착 인정 거리
    public float safetyLineZ = -5.0f;     // 안전선 위치 (이보다 크면 경고)

    // 데이터 기록 변수
    private float totalTime = 0f;
    private float safeTime = 0f;
    private int hitCount = 0;
    private float lastLogTime = 0f;       // 로그 도배 방지용

    void Start()
    {
        // 시작 시 결과창 숨기기
        if (resultUIPanel != null) resultUIPanel.SetActive(false);
        if (mapCamera != null) mapCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        // 상태에 따라 다른 로직 수행
        switch (currentState)
        {
            case TrainingState.Ready:
                CheckReadyState();
                break;
            case TrainingState.Training:
                CheckTrainingState();
                break;
            case TrainingState.Finished:
                // 종료 상태에서는 입력 대기 (InputHandler가 처리)
                break;
        }
    }

    // --- [1] 준비 상태 로직 ---
    void CheckReadyState()
    {
        // 부정출발 감시 (안전선 넘었나?)
        if (IsOverLine())
        {
            if (Time.time - lastLogTime > 1.0f)
            {
                Debug.Log("⛔ 위험! 훈련 전입니다. 뒤로 물러나세요! (Back)");
                lastLogTime = Time.time;
            }
        }
    }

    // --- [2] 훈련 중 로직 ---
    void CheckTrainingState()
    {
        // 시간 측정
        totalTime += Time.deltaTime;

        // 직선 보행 점수 계산 (이탈 안 했을 때만 시간 적립)
        if (detector.IsDeviated)
        {
            // 이탈 중에는 safeTime이 오르지 않음
            // (필요하다면 여기서 실시간 경고 사운드 재생 가능)
            //Debug.Log($"⚠️ 경고! {detector.DirectionFeedback} 쪽으로 이동하세요!");
        }
        else
        {
            safeTime += Time.deltaTime;
        }

        // 도착 체크
        CheckArrival();
    }

    // --- 내부 기능 함수들 ---

    void CheckArrival()
    {
        // 높이 무시, 수평 거리만 계산
        float dist = Vector3.Distance(
            new Vector3(userHMD.position.x, 0, userHMD.position.z),
            new Vector3(endPoint.position.x, 0, endPoint.position.z)
        );

        float finishLineZ = endPoint.position.z - arrivalDistance;

        // 현재 내 위치가 피니쉬 라인보다 커졌다면 (지나갔다면)
        if (userHMD.position.z >= finishLineZ)
        {
            EndTraining();
        }
    }

    void EndTraining()
    {
        currentState = TrainingState.Finished;
        detector.StopSensor(); // 센서 끄기

        if (pathVisualizer != null) pathVisualizer.StopDrawing();

        // 점수 계산 (백분율)
        float score = 0f;
        if (totalTime > 0) score = (safeTime / totalTime) * 100f;

        // UI 및 오디오 활성화
        string finalReport = GenerateReportString(score);
        ShowResultUI(finalReport);

    }

    // 3. 리포트 문구 생성 (StringBuilder 사용으로 성능/가독성 향상)
    string GenerateReportString(float score)
    {
        string feedbackMsg = (score >= 80)
            ? "아주 훌륭합니다! 완벽에 가깝습니다."
            : "조금 더 직선 유지 연습이 필요합니다.";

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"총 소요 시간    : {totalTime:F2}초");
        sb.AppendLine($"직선 보행 유지율: {score:F1}%");
        sb.AppendLine($"장애물 충돌     : {hitCount}회");
        sb.AppendLine($"{feedbackMsg}");

        return sb.ToString();
    }

    // 4. UI 활성화 및 텍스트 적용
    void ShowResultUI(string message)
    {
        if (resultUIPanel != null)
        {
            resultUIPanel.SetActive(true);

            // 미니맵 카메라 켜기
            if (mapCamera != null) mapCamera.gameObject.SetActive(true);

            // 텍스트 적용
            if (reportText != null) reportText.text = message;

            if (audioSource && guideVoiceClip) audioSource.PlayOneShot(guideVoiceClip);
        }
    }
    public bool IsOverLine()
    {
        // 플레이어의 실제 월드 Z 위치
        float currentWorldZ = userHMD.position.z;

        // "앞으로 가는 방향(Z가 커지는 방향)"이 훈련 방향이므로
        // 현재 위치가 설정한 17보다 커지면 넘은 것입니다.
        return currentWorldZ > safetyLineZ;
    }

    // --- [중요] 외부(InputHandler/Obstacle)에서 호출할 함수들 ---

    // 1. 훈련 시작 시도 (InputHandler가 호출)
    public void TryStartTraining()
    {
        if (currentState != TrainingState.Ready) return;

        if (IsOverLine())
        {
            Debug.Log("❌ 시작 불가: 안전선 뒤로 물러나야 합니다.");
        }
        else
        {
            currentState = TrainingState.Training;
            // 변수 초기화
            totalTime = 0f;
            safeTime = 0f;
            hitCount = 0;

            detector.StartSensor();

            if (pathVisualizer != null) pathVisualizer.StartDrawing();
            Debug.Log("🚀 훈련 프로세스 시작! (목표 지점으로 이동하세요)");
        }
    }

    // 2. 장애물 충돌 기록 (장애물 스크립트가 호출)
    public void AddHitCount()
    {
        if (currentState == TrainingState.Training)
        {
            hitCount++;
            Debug.Log($"💥 충돌! (누적 {hitCount}회)");
        }
    }

    // 3. 재시작 (InputHandler가 호출)
    public void RetryGame()
    {
        if (currentState == TrainingState.Finished)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // 4. 타이틀로 이동 (InputHandler가 호출)
    public void GoTitle()
    {
        if (currentState == TrainingState.Finished)
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}