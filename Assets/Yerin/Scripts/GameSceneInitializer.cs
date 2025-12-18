using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("========================================");
        Debug.Log("[GameScene] 초기화 시작!");

        // PlayerPrefs에서 비전 효과 로드
        int savedVision = PlayerPrefs.GetInt("SelectedVision", -1);

        if (savedVision == -1)
        {
            Debug.LogWarning("[GameScene] ⚠️ SelectedVision이 저장되지 않음! 기본값 0 사용");
            savedVision = 0;
        }

        Debug.Log($"[GameScene] 로드된 비전 타입: {savedVision}");
        Debug.Log($"[GameScene] 0=Normal, 1=Low, 2=Tunnel, 3=Peripheral");

        // VisionEffectManager 찾기
        VisionEffectManager visionManager = FindObjectOfType<VisionEffectManager>();

        if (visionManager != null)
        {
            Debug.Log($"[GameScene] ✅ VisionEffectManager 찾음!");
            visionManager.SetVisionEffect(savedVision);
        }
        else
        {
            Debug.LogError("[GameScene] ❌ VisionEffectManager를 찾을 수 없습니다!");
            Debug.LogError("[GameScene] Main Camera에 VisionEffectManager 추가하세요!");
        }

        Debug.Log("========================================");
    }
}