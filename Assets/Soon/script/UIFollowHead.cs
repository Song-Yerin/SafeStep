using UnityEngine;

public class UIFollowHead : MonoBehaviour
{
    [Header("설정")]
    public Transform cameraTransform;   // 메인 카메라 (User Head)
    public float distance = 2.0f;       // 눈앞 2미터 거리에 유지
    public float smoothSpeed = 5.0f;    // 따라오는 속도 (클수록 빠름)

    [Header("높이 조절")]
    public float heightOffset = 0.0f;   // 위아래 위치 보정

    void Update()
    {
        if (cameraTransform == null) return;

        // 1. 목표 위치 계산 (카메라가 바라보는 방향 앞쪽 distance 만큼)
        // 카메라의 위치 + (카메라 앞방향 * 거리)
        Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distance);

        // 높이 보정 (필요하다면)
        targetPosition.y += heightOffset;

        // 2. 부드럽게 이동 (Lerp 사용)
        // 중요: Time.unscaledDeltaTime을 써야 시간이 멈춰도(Pause) UI는 움직임!
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * smoothSpeed);

        // 3. 항상 카메라를 바라보게 회전 (Billboarding)
        // UI가 카메라를 정면으로 보게 함
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraTransform.rotation, Time.unscaledDeltaTime * smoothSpeed);
    }
}