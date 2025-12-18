using UnityEngine;
using UnityEngine.InputSystem; // 인풋 시스템 필수

public class TrainingInputHandler : MonoBehaviour
{
    [Header("매니저 연결")]
    public TrainingManager manager; // 명령을 내릴 대상

    [Header("입력 소스 설정")]
    public InputActionProperty moveInputSource; // 조이스틱 (종료 메뉴용)
    public InputActionProperty startInputSource; // 버튼 (시작용, 예: Trigger or Button A)
    // 키보드 테스트용으로 스페이스바도 유지하려면 코드에 남겨둠

    private bool inputDelay = false; // 중복 입력 방지

    void Update()
    {
        // 1. 준비 상태일 때 -> 시작 입력 감시
        if (manager.currentState == TrainingManager.TrainingState.Ready)
        {
            // 키보드 Space OR VR 컨트롤러 버튼 누르면
            if (Input.GetKeyDown(KeyCode.Space) || IsVRButtonPressed())
            {
                manager.TryStartTraining();
            }
        }

        // 2. 종료 상태일 때 -> 조이스틱 입력 감시
        else if (manager.currentState == TrainingManager.TrainingState.Finished)
        {
            HandleMenuInput();
        }
    }

    // VR 컨트롤러 시작 버튼 감지 (예시: 트리거 버튼)
    bool IsVRButtonPressed()
    {
        // startInputSource에 연결된 버튼이 눌렸는지 확인 (값 0.5 이상이면 눌림)
        return startInputSource.action != null && startInputSource.action.ReadValue<float>() > 0.5f;
    }

    // 종료 메뉴 조이스틱 처리
    void HandleMenuInput()
    {
        if (inputDelay) return;

        Vector2 input = moveInputSource.action.ReadValue<Vector2>();

        // 왼쪽 (다시하기)
        if (input.x < -0.8f)
        {
            Debug.Log("🕹️ 왼쪽 입력 -> 재시작");
            manager.RetryGame();
            inputDelay = true;
        }
        // 오른쪽 (타이틀)
        else if (input.x > 0.8f)
        {
            Debug.Log("🕹️ 오른쪽 입력 -> 타이틀");
            manager.GoTitle();
            inputDelay = true;
        }
    }
}