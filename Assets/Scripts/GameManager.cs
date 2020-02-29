using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool levelFailed = false;

    public float restartLevelDelay = 1f;

    Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player fell off a platform
        if (player != null && player.position.y < -2f)
        {
            LevelFailed();
            return;
        }

        // "R" for restarting active level
        if (Input.GetKey(KeyCode.R))
        {
            RestartActiveLevel();
            return;
        }
    }

    public void CompleteLevel()
    {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextLevelIndex);
    }

    void LevelFailed()
    {
        if (levelFailed)
        {
            return;
        }

        levelFailed = true;
        Invoke("RestartActiveLevel", restartLevelDelay);
    }

    void RestartActiveLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
