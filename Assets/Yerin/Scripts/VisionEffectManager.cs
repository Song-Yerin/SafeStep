using UnityEngine;
using UnityEngine.Rendering;

public class VisionEffectManager : MonoBehaviour
{
    [Header("Volume")]
    public Volume globalVolume;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Vision Profiles")]
    public VolumeProfile blindnessProfile;      // 0: Color + Bloom
    public VolumeProfile lowVisionProfile;       // 1: Depth + Bloom
    public VolumeProfile tunnelVisionProfile;    // 2: Depth + Vignette
    public VolumeProfile peripheralVisionProfile; // 3: Depth

    public int currentVisionType = 0;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (globalVolume == null)
            globalVolume = FindObjectOfType<Volume>();

        if (globalVolume != null)
        {
            Debug.Log($"[VisionEffect] ✅ Volume 찾음!");
        }
        else
        {
            Debug.LogError("[VisionEffect] ❌ Global Volume 없음!");
        }
    }

    public void SetVisionEffect(int visionType)
    {
        currentVisionType = visionType;

        Debug.Log($"==========================================");
        Debug.Log($"[VisionEffect] 비전 타입 {visionType} 적용!");

        if (globalVolume == null)
        {
            Debug.LogError("[VisionEffect] Global Volume이 없습니다!");
            return;
        }

        // Profile 교체 + FOV 변경
        switch (visionType)
        {
            case 0: // Blindness - Color + Bloom
                ApplyBlindness();
                break;

            case 1: // Low Vision - Depth + Bloom
                ApplyLowVision();
                break;

            case 2: // Tunnel Vision - Depth + Vignette
                ApplyTunnelVision();
                break;

            case 3: // Peripheral Vision - Depth
                ApplyPeripheralVision();
                break;
        }

        Debug.Log($"[VisionEffect] Profile: {globalVolume.profile.name}");
        Debug.Log($"[VisionEffect] FOV: {mainCamera.fieldOfView}");
        Debug.Log($"==========================================");
    }

    void ApplyBlindness()
    {
        if (blindnessProfile != null)
        {
            globalVolume.profile = blindnessProfile;
        }

        if (mainCamera != null)
            mainCamera.fieldOfView = 60f;

        Debug.Log("[VisionEffect] ⚫ 완전 블라인드 (Color + Bloom)");
    }

    void ApplyLowVision()
    {
        if (lowVisionProfile != null)
        {
            globalVolume.profile = lowVisionProfile;
        }

        if (mainCamera != null)
            mainCamera.fieldOfView = 55f;

        Debug.Log("[VisionEffect] 🔵 저시력 (Depth + Bloom)");
    }

    void ApplyTunnelVision()
    {
        if (tunnelVisionProfile != null)
        {
            globalVolume.profile = tunnelVisionProfile;
        }

        if (mainCamera != null)
            mainCamera.fieldOfView = 20f; // 매우 좁게

        Debug.Log("[VisionEffect] 🔴 터널 시야 (Depth + Vignette + FOV 20)");
    }

    void ApplyPeripheralVision()
    {
        if (peripheralVisionProfile != null)
        {
            globalVolume.profile = peripheralVisionProfile;
        }

        if (mainCamera != null)
            mainCamera.fieldOfView = 100f; // 매우 넓게

        Debug.Log("[VisionEffect] 🟡 주변 시야 (Depth + FOV 100)");
    }
}