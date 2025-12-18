using UnityEngine;
using UnityEngine.SceneManagement;

public class VisionSelectionManager : MonoBehaviour
{
    [Header("씬 이름")]
    public string mapSelectionSceneName = "MapSelectionScene";



    // 🔴 각 버튼의 OnClick에 연결
    public void SelectNormalVision()
    {
        SaveVisionAndGoToMapSelection(VisionType.Normal);
    }

    public void SelectLowVision()
    {
        SaveVisionAndGoToMapSelection(VisionType.LowVision);
    }

    public void SelectTunnelVision()
    {
        SaveVisionAndGoToMapSelection(VisionType.TunnelVision);
    }

    public void SelectPeripheralVision()
    {
        SaveVisionAndGoToMapSelection(VisionType.PeripheralVision);
    }

    void SaveVisionAndGoToMapSelection(VisionType visionType)
    {
        // 선택한 비전 효과 저장
        PlayerPrefs.SetInt("SelectedVision", (int)visionType);
        PlayerPrefs.Save();

        Debug.Log($"[VisionSelection] {visionType} 선택됨 → 맵 선택 씬으로 이동");

        // 맵 선택 씬으로 이동
        SceneManager.LoadScene(mapSelectionSceneName);
    }

    public void OnButtonClick()
    {
        Debug.Log($"Vision 선택됨!");

        PlayerPrefs.Save();

        SceneManager.LoadScene(mapSelectionSceneName);
    }
}

public enum VisionType
{
    Normal = 0,
    LowVision = 1,
    TunnelVision = 2,
    PeripheralVision = 3
}