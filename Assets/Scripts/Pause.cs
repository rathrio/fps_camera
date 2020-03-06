using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject screen;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Hide();
    }

    void Hide()
    {
        screen.SetActive(false);
    }

    void Show()
    {
        screen.SetActive(true);
    }

    public void PauseGame()
    {
        Debug.Log("Hit Pause");

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Show();
    }

    public void ResumeGame()
    {
        Debug.Log("Hit Resume");

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Hide();
    }

    bool IsPaused()
    {
        return Time.timeScale == 0;
    }

    void TogglePause()
    {
        if (IsPaused())
        {
            ResumeGame();
        } else
        {
            PauseGame();
        }
    }

    private void Update()
    {
        // "ESC" for toggling PAUSE Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
}
