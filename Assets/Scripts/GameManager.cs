using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float restartLevelDelay = 1f;

    // Hacky global storage for Demo purposes
    public Dictionary<int, float> completionTimes = new Dictionary<int, float>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
        // "R" for restarting active level
        if (Input.GetKey(KeyCode.R))
        {
            RestartActiveLevel();
            return;
        }

    }

    public void CompleteLevel()
    {
        completionTimes[CurrentBuildIndex()] =  Time.timeSinceLevelLoad;
        int nextLevelIndex = CurrentBuildIndex() + 1;
        SceneManager.LoadScene(nextLevelIndex);
    }

    private static int CurrentBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void LevelFailed()
    {
        Invoke("RestartActiveLevel", restartLevelDelay);
    }

    public void RestartActiveLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
