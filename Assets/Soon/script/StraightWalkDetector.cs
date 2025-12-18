using UnityEngine;

public class StraightWalkDetector : MonoBehaviour
{
    [Header("필수 연결")]
    public Transform userHMD;   // 플레이어 카메라
    public Transform endPoint;  // 도착점 (Destination)
    public Transform crosswalkStartPoint; // 횡단보도 시작점 (안전선 기준용)

    [Header("오디오 연결 (추가됨)")]
    public AudioSource audioSource;
    public AudioClip goLeftClip;  // "왼쪽으로 이동하세요" (오른쪽으로 이탈했을 때)
    public AudioClip goRightClip; // "오른쪽으로 이동하세요" (왼쪽으로 이탈했을 때)

    [Header("감도 설정")]
    public float warningThreshold = 0.5f; // 이탈 허용 범위 (미터)
    public bool useRelativePath = true;   // 출발 위치 기준 여부

    // --- 외부 정보 ---
    public bool IsDeviated { get; private set; }
    public float CurrentDeviation { get; private set; }
    public string DirectionFeedback { get; private set; }

    // 내부 변수
    private Vector3 pathStartPos;
    private Vector3 pathDirection;
    private bool isMonitoring = false;
    private float lastLogTime = 0f;
    private float audioCooldown = 2.5f; // 음성 안내 간격 (너무 자주 울리면 시끄러움)

    public void StartSensor()
    {
        // 1. 방향 벡터 계산 (시작점 -> 도착점)
        Vector3 startPos = (crosswalkStartPoint != null) ? crosswalkStartPoint.position : userHMD.position;
        Vector3 endPos = endPoint.position;

        Vector3 direction = endPos - startPos;
        direction.y = 0;
        pathDirection = direction.normalized;

        // 2. 경로의 기준점 설정
        if (useRelativePath)
        {
            pathStartPos = userHMD.position;
        }
        else
        {
            pathStartPos = startPos;
        }
        pathStartPos.y = 0;

        isMonitoring = true;
        IsDeviated = false;
        Debug.Log($"📡 센서 작동 시작. 기준: {pathStartPos}");
    }

    public void StopSensor()
    {
        isMonitoring = false;
        IsDeviated = false;
    }

    void Update()
    {
        if (!isMonitoring) return;

        // 1. 현재 위치 계산
        Vector3 currentUserPos = userHMD.position;
        currentUserPos.y = 0;

        // 2. 이탈 거리 계산 (외적 활용)
        Vector3 vectorToUser = currentUserPos - pathStartPos;
        CurrentDeviation = Vector3.Cross(pathDirection, vectorToUser).y;

        // 3. 판정 로직
        if (Mathf.Abs(CurrentDeviation) > warningThreshold)
        {
            IsDeviated = true;

            // 쿨타임 체크 (로그와 음성이 너무 자주 나오지 않게)
            if (Time.time - lastLogTime > audioCooldown)
            {
                if (CurrentDeviation > 0)
                {
                    // 오른쪽(+)으로 치우침 -> "왼쪽으로 가세요"
                    DirectionFeedback = "Left";
                    Debug.Log($"⚠️ 오른쪽으로 이탈! (◀️ 왼쪽으로 이동하세요)");
                    PlayGuideVoice(goLeftClip);
                }
                else
                {
                    // 왼쪽(-)으로 치우침 -> "오른쪽으로 가세요"
                    DirectionFeedback = "Right";
                    Debug.Log($"⚠️ 왼쪽으로 이탈! (▶️ 오른쪽으로 이동하세요)");
                    PlayGuideVoice(goRightClip);
                }
                lastLogTime = Time.time;
            }
        }
        else
        {
            IsDeviated = false;
            DirectionFeedback = "Straight";
        }
    }

    // 음성 재생 함수 (중복 재생 방지 포함)
    void PlayGuideVoice(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // 이미 말하고 있으면 끊지 않고 기다릴지, 아니면 덮어쓸지 결정
            // 여기서는 중요한 경고이므로 즉시 재생 (PlayOneShot)
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (isMonitoring)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pathStartPos, pathStartPos + pathDirection * 20f);
        }
    }
}