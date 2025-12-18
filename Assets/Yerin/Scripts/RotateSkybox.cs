using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private bool autoRotate = true;

    [Header("Manual Control")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Space;

    private float currentRotation = 0f;

    private void Update()
    {
        // 일시정지 토글
        if (Input.GetKeyDown(pauseKey))
        {
            autoRotate = !autoRotate;
        }

        if (autoRotate)
        {
            currentRotation += Time.deltaTime * rotationSpeed;
            currentRotation %= 360f; // 0-360 범위로 유지
            RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
        }
    }
}