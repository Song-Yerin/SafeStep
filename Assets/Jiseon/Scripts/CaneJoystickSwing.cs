using UnityEngine;
using UnityEngine.InputSystem;

public class CaneJoystickSwing : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference rightJoystickAction;

    [Header("Cane Root")]
    public Transform virtualCaneRoot;

    [Header("Rotation Settings")]
    public float rotationSpeed = 60f;   // 회전 속도 (deg/sec)
    public float maxAngle = 30f;         // 최대 좌우 각도

    float currentAngle = 0f;

    void OnEnable()
    {
        rightJoystickAction.action.Enable();
    }

    void OnDisable()
    {
        rightJoystickAction.action.Disable();
    }

    void Update()
    {
        Vector2 joystick = rightJoystickAction.action.ReadValue<Vector2>();

        // Y축만 사용 (앞/뒤)
        float inputY = joystick.y;

        // 사용자 기준:
        // 조이스틱 ↑  → -X 회전
        // 조이스틱 ↓  → +X 회전
        currentAngle += -inputY * rotationSpeed * Time.deltaTime;

        // 각도 제한
        currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);

        // X축 회전 적용
        virtualCaneRoot.localRotation = Quaternion.Euler(
            currentAngle,
            0f,
            0f
        );
    }
}
