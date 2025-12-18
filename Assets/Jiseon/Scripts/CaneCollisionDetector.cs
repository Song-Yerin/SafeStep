using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class CaneCollisionDetector : MonoBehaviour
{
    [Header("Haptics")]
    public HapticImpulsePlayer hapticPlayer;
    public float groundAmplitude = 0.05f;
    public float guideAmplitude = 0.12f;
    public float obstacleAmplitude = 0.7f;
    public float hapticDuration = 0.05f;
    public float hapticInterval = 0.15f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tactileGuideClip;   // 보도블록 질감 소리
    public AudioClip tactileWarningClip; // 경고블록 소리
    public AudioClip obstacleClip;        // 자전거/킥보드 충돌음

    float hapticTimer = 0f;
    bool isOnGuideBlock = false;
    bool isOnObstacle = false;

    void Update()
    {
        hapticTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (hapticTimer > 0f)
            return;

        HandleCollision(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tactile"))
            isOnGuideBlock = false;

        if (other.CompareTag("Obstacle"))
            isOnObstacle = false;
    }

    void HandleCollision(Collider other)
    {
        // 1. 일반 바닥
        if (other.CompareTag("Ground"))
        {
            PlayHaptic(groundAmplitude);
            return;
        }

        // 2. 보도블록
        if (other.CompareTag("Tactile"))
        {
            var tactile = other.GetComponent<TactileBlockInfo>();
            if (tactile == null) return;

            if (tactile.type == TactileType.Guide)
            {
                isOnGuideBlock = true;
                PlayHaptic(guideAmplitude);
                PlaySound(tactileGuideClip);
            }
            else if (tactile.type == TactileType.Warning)
            {
                StartCoroutine(WarningHapticPattern());
                PlaySound(tactileWarningClip);
            }
            return;
        }

        // 3. 장애물 (킥보드 / 자전거)
        if (other.CompareTag("Obstacle"))
        {
            isOnObstacle = true;
            PlayHaptic(obstacleAmplitude);
            PlaySound(obstacleClip);
        }
    }

    void PlayHaptic(float amplitude)
    {
        if (hapticPlayer == null)
            return;

        hapticPlayer.SendHapticImpulse(amplitude, hapticDuration);
        hapticTimer = hapticInterval;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(clip);
    }

    IEnumerator WarningHapticPattern()
    {
        // 경고블록: 강한 진동 3회
        for (int i = 0; i < 3; i++)
        {
            hapticPlayer.SendHapticImpulse(0.7f, 0.05f);
            yield return new WaitForSeconds(0.1f);
        }

        hapticTimer = hapticInterval;
    }
}
