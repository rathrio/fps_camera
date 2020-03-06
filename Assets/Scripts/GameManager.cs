using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float restartLevelDelay = 1f;

    // Hacky global storage for Demo purposes
    public static Dictionary<int, float> completionTimes = new Dictionary<int, float>();

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartActiveLevel();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadLevel(1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadLevel(2);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadLevel(3);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            LoadLevel(4);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            LoadLevel(5);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            LoadLevel(6);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            LoadLevel(7);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            LoadLevel(8);
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

    void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
