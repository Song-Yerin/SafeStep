using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;  // 🔴 URP용

public class VisionEffectManager : MonoBehaviour
{
    public Volume postProcessVolume;
    public int currentVisionType = 0;

    private Vignette vignette;
    private DepthOfField depthOfField;
    private ColorAdjustments colorAdjustments;
    private Bloom bloom;

    void Start()
    {
        if (postProcessVolume == null)
            postProcessVolume = FindObjectOfType<Volume>();

        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out depthOfField);
            postProcessVolume.profile.TryGet(out colorAdjustments);
            postProcessVolume.profile.TryGet(out bloom);
        }
    }

    public void SetVisionEffect(int visionType)
    {
        currentVisionType = visionType;
        ResetEffects();

        switch (visionType)
        {
            case 0:
                ApplyNormalVision();
                break;
            case 1:
                ApplyLowVision();
                break;
            case 2:
                ApplyTunnelVision();
                break;
            case 3:
                ApplyPeripheralVision();
                break;
        }

        Debug.Log($"[VisionEffect] {visionType} 적용됨");
    }

    void ResetEffects()
    {
        if (vignette != null)
            vignette.active = false;
        if (depthOfField != null)
            depthOfField.active = false;
        if (bloom != null)
            bloom.active = false;
    }

    void ApplyNormalVision()
    {
        Debug.Log("[VisionEffect] 정상 시력");
    }

    void ApplyLowVision()
    {
        if (bloom != null)
        {
            bloom.active = true;
            bloom.intensity.value = 5f;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = -30f;
        }

        Debug.Log("[VisionEffect] 저시력");
    }

    void ApplyTunnelVision()
    {
        if (vignette != null)
        {
            vignette.active = true;
            vignette.intensity.value = 0.6f;
            vignette.smoothness.value = 0.4f;
        }

        Debug.Log("[VisionEffect] 터널 시야");
    }

    void ApplyPeripheralVision()
    {
        if (depthOfField != null)
        {
            depthOfField.active = true;
            depthOfField.focusDistance.value = 100f;
        }

        Debug.Log("[VisionEffect] 주변 시야");
    }
}