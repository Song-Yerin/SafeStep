// VisionEffectManager.cs - 각 맵 씬에 배치
using UnityEngine;

public class VisionEffectManager : MonoBehaviour
{
    [Header("Effect Materials")]
    public Material lowVisionMaterial;
    public Material tunnelVisionMaterial;
    public Material completeBlindnessMaterial;
    public Material peripheralLossMaterial;

    private Camera mainCamera;
    private VisionType currentVisionType;

    private void Start()
    {
        // 맵 씬에 들어왔을 때 GameManager에서 선택된 타입 가져오기
        if (GameManager.Instance != null)
        {
            currentVisionType = GameManager.Instance.selectedVisionType;
            Debug.Log($"Applying vision effect: {currentVisionType}");
        }
        else
        {
            Debug.LogWarning("GameManager not found! Using default vision type.");
            currentVisionType = VisionType.LowVision;
        }

        FindAndApplyEffect();
    }

    private void FindAndApplyEffect()
    {
        // VR 카메라 찾기
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            GameObject xrRig = GameObject.Find("XR Rig") ?? GameObject.Find("XR Origin");
            if (xrRig != null)
            {
                mainCamera = xrRig.GetComponentInChildren<Camera>();
            }
        }

        if (mainCamera != null)
        {
            ApplyVisionEffect();
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void ApplyVisionEffect()
    {
        Material selectedMaterial = null;

        switch (currentVisionType)
        {
            case VisionType.LowVision:
                selectedMaterial = lowVisionMaterial;
                break;
            case VisionType.TunnelVision:
                selectedMaterial = tunnelVisionMaterial;
                break;
            case VisionType.CompleteBlindness:
                selectedMaterial = completeBlindnessMaterial;
                break;
            case VisionType.PeripheralVisionLoss:
                selectedMaterial = peripheralLossMaterial;
                break;
        }

        if (selectedMaterial != null)
        {
            // VisionEffectRenderer 추가 또는 업데이트
            VisionEffectRenderer renderer = mainCamera.GetComponent<VisionEffectRenderer>();
            if (renderer == null)
            {
                renderer = mainCamera.gameObject.AddComponent<VisionEffectRenderer>();
            }
            renderer.effectMaterial = selectedMaterial;

            Debug.Log($"Vision effect applied: {currentVisionType}");
        }
        else
        {
            Debug.LogError($"Material for {currentVisionType} is not assigned!");
        }
    }

    // 개발자 테스트용 - 런타임에 효과 변경
    public void ChangeVisionEffect(VisionType newType)
    {
        currentVisionType = newType;
        ApplyVisionEffect();
    }
}