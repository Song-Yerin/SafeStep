using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyClickToStart : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip welcomeTTS;
    [SerializeField] private string nextSceneName = "MainMenu";

    private bool hasStarted = false;

    void Start()
    {
        if (audioSource && welcomeTTS)
        {
            audioSource.PlayOneShot(welcomeTTS);
        }
    }

    void Update()
    {
        if (hasStarted) return;

        // 아무 입력이나 감지
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        hasStarted = true;
        SceneManager.LoadScene(nextSceneName);
    }
}