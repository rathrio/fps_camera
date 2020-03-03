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
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Show();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Hide();
    }

    private void Update()
    {
        // "ESC" for PAUSE Menu
        if (Input.GetKey(KeyCode.Escape))
        {
            PauseGame();
        }
    }
}
