using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float restartLevelDelay = 1f;

    Transform player;
    bool levelFailed = false;

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
        FindObjectOfType<Score>().completionTimes.Add(CurrentBuildIndex(), Time.timeSinceLevelLoad);
        int nextLevelIndex = CurrentBuildIndex() + 1;
        SceneManager.LoadScene(nextLevelIndex);
    }

    private static int CurrentBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
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

    public void RestartActiveLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
