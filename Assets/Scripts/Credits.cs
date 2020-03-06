using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Text;

public class Credits : MonoBehaviour
{
    public Text completionTimes;

    readonly static Dictionary<int, float> RADIS_BEST_TIMES = new Dictionary<int, float>
    {
        { 1, 1.2f },
        { 2, 4.1f },
        { 3, 6.1f },
        { 4, 3.2f },
        { 5, 4.1f },
        { 6, 1.7f },
        { 7, 5.0f },
        { 8, 12.4f },
    };

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StringBuilder sb = new StringBuilder();
        foreach(KeyValuePair<int, float> score in GameManager.completionTimes)
        {
            int levelIndex = score.Key;
            String seconds = score.Value.ToString("F1") + "s\t" + "(" + "Radi: " + RADIS_BEST_TIMES[levelIndex] + "s)";

            sb.AppendLine("Level " + levelIndex + ": " + seconds);
        }

        completionTimes.text = sb.ToString();
    }

    public void RestartGame()
    {
        FindObjectOfType<GameManager>().StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
