using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MapSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Map Info")]
    [SerializeField] private string mapName;
    [SerializeField] private string nextSceneName;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverTTS;
    [SerializeField] private AudioClip clickTTS;

    [Header("Haptic Feedback")]
    [SerializeField] private float hapticIntensity = 0.5f;
    [SerializeField] private float hapticDuration = 0.1f;

    private bool isHovering = false; // hasHovered 대신 isHovering

    void Start()
    {
        Debug.Log($"MapSelectButton [{mapName}] initialized");

        if (!audioSource)
            Debug.LogError("AudioSource missing!");
        if (!hoverTTS)
            Debug.LogWarning("Hover TTS missing!");
    }

    // 가리킬 때마다 실행
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Pointer Enter: {mapName}");

        if (!isHovering) // 중복 방지
        {
            isHovering = true;

            if (audioSource && hoverTTS)
            {
                Debug.Log("Playing hover sound");
                audioSource.PlayOneShot(hoverTTS);
            }

            TriggerHaptic();
        }
    }

    // 떼면 다시 리셋
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Pointer Exit: {mapName}");
        isHovering = false; // 떼면 다시 false로
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked: {mapName}");

        if (audioSource && clickTTS)
        {
            audioSource.PlayOneShot(clickTTS);
        }

        TriggerHaptic(0.8f, 0.2f);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            float delay = clickTTS ? clickTTS.length : 0.5f;
            StartCoroutine(LoadSceneAfterDelay(delay));
        }
    }

    void TriggerHaptic(float intensity = -1, float duration = -1)
    {
        if (intensity < 0) intensity = hapticIntensity;
        if (duration < 0) duration = hapticDuration;

        Debug.Log("Triggering haptic feedback");

        var controllers = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.ActionBasedController>();
        foreach (var controller in controllers)
        {
            controller.SendHapticImpulse(intensity, duration);
        }
    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}