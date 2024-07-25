using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject pauseMenuUI; // UI 오브젝트를 연결할 필드

    private bool isPaused = false;

    void Awake()
    {
        // 싱글톤 패턴 구현
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

    void Update()
    {
        if (ManagingInput.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // 게임 정지
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // 게임 재개
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 시간 스케일을 원래대로 되돌립니다.
        pauseMenuUI.SetActive(false);
        SceneManager.LoadScene("Lobby"); // 로비 씬 재시작
    }

    public void QuitGame()
    {
        pauseMenuUI.SetActive(false);
        Application.Quit(); // 게임 종료
    }
}