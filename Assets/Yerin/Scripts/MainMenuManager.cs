using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip welcomeTTS; // "훈련할 환경을 선택해주세요"

    void Start()
    {
        // 메뉴 진입하자마자 TTS 재생
        if (audioSource && welcomeTTS)
        {
            audioSource.PlayOneShot(welcomeTTS);
        }
    }
}