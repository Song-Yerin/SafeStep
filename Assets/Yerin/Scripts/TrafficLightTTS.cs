using UnityEngine;
using System.Collections;

public class TrafficLightTTS : MonoBehaviour
{
    [Header("TTS 오디오 클립")]
    public AudioClip greenTTS;   // "초록불입니다. 건너가세요"
    public AudioClip redTTS;     // "빨간불입니다. 기다리세요"
    public AudioClip yellowTTS;  // "노란불입니다"

    [Header("반복 설정")]
    public bool repeatWhileGreen = true;   // 초록불 동안 반복
    public bool repeatWhileRed = false;    // 빨간불 동안 반복
    public float repeatInterval = 3f;      // 반복 간격 (초)

    private AudioSource audioSource;
    private LightState currentState;
    private Coroutine repeatCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 3D 사운드 설정
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 10f;
        audioSource.maxDistance = 50f;
        audioSource.loop = false;
    }

    // 🔴 외부에서 호출 (IntersectionManager에서)
    public void OnLightStateChanged(LightState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // 기존 반복 중지
        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine);
            repeatCoroutine = null;
        }

        // 상태별 처리
        switch (newState)
        {
            case LightState.Green:
                PlayTTS(greenTTS);
                if (repeatWhileGreen && greenTTS != null)
                {
                    repeatCoroutine = StartCoroutine(RepeatTTS(greenTTS, repeatInterval));
                }
                break;

            case LightState.Yellow:
                PlayTTS(yellowTTS);
                break;

            case LightState.Red:
                PlayTTS(redTTS);
                if (repeatWhileRed && redTTS != null)
                {
                    repeatCoroutine = StartCoroutine(RepeatTTS(redTTS, repeatInterval));
                }
                break;
        }
    }

    void PlayTTS(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log($"[TTS] 재생: {clip.name}");
        }
    }

    IEnumerator RepeatTTS(AudioClip clip, float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            PlayTTS(clip);
        }
    }
}