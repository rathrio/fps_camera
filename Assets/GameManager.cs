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
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player fell off a platform
        if (player.position.y < 0f)
        {
            LevelFailed();
        }
    }

    public void CompleteLevel()
    {
        RestartActiveLevel();
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
