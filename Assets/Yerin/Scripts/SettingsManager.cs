// SettingsManager.cs - Settings 씬에서
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button lowVisionButton;
    public Button tunnelVisionButton;
    public Button completeBlindnessButton;
    public Button peripheralVisionLossButton;

    private void Start()
    {
        lowVisionButton.onClick.AddListener(() => SelectVisionType(VisionType.LowVision));
        tunnelVisionButton.onClick.AddListener(() => SelectVisionType(VisionType.TunnelVision));
        completeBlindnessButton.onClick.AddListener(() => SelectVisionType(VisionType.CompleteBlindness));
        peripheralVisionLossButton.onClick.AddListener(() => SelectVisionType(VisionType.PeripheralVisionLoss));
    }

    private void SelectVisionType(VisionType type)
    {
        // 선택만 저장하고 MainMenu로 이동
        GameManager.Instance.selectedVisionType = type;
        Debug.Log($"Selected Vision Type: {type} - Will be applied in actual map");

        SceneManager.LoadScene("MainMenu");
    }
}
