// GameManager.cs - 선택 정보만 저장
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public VisionType selectedVisionType = VisionType.LowVision;
    public string selectedMap = ""; // 선택된 맵 이름

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}