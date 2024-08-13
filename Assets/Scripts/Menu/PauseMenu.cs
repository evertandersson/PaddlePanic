using UnityEngine;
using UnityEngine.SceneManagement;
using static MainMenuButtons;

public class PauseMenu : MonoBehaviour
{
    public Transform playerStartTransform;
    public bool isPaused = false;
    public GameObject player;

    public delegate void OnMenuClicked();
    public static OnMenuClicked onMenuClicked;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void OnEnable()
    {
        onMenuClicked += ExitToMenu;
    }
    private void OnDisable()
    {
        onMenuClicked -= ExitToMenu;
    }

    public void OnPausePressed()
    {
        isPaused = true;
        Time.timeScale = 0;
    }
    public void OnResumePressed()
    {
        isPaused = false;
        Time.timeScale = 1;
    }
    public void OnRestartPressed()
    {
        isPaused = false;
        player.transform.parent.SetPositionAndRotation(playerStartTransform.gameObject.transform.position,playerStartTransform.gameObject.transform.rotation);
        Time.timeScale = 1; 

        gameObject.SetActive(false);
        EventManager.Raise(EventKey.RACE_START, new Void());
    }
    public void OnRestartEndLevelPressed()
    {
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void OnMenuPressed()
    {
        onMenuClicked?.Invoke();
    }

    private void ExitToMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        Destroy((GameObject)player);
        SceneManager.LoadScene("MainMenuScene(Backup)");
    }
}
