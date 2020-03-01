using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Text;

public class Credits : MonoBehaviour
{
    public Text completionTimes;

    static Dictionary<int, float> RADIS_BEST_TIMES = new Dictionary<int, float>
    {
        { 1, 3.14f },
        { 2, 3.14f },
        { 3, 3.14f },
        { 4, 3.14f },
        { 5, 3.14f },
        { 6, 3.14f },
        { 7, 3.14f },
        { 8, 3.14f },
    };

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StringBuilder sb = new StringBuilder();
        foreach(KeyValuePair<int, float> score in FindObjectOfType<Score>().completionTimes)
        {
            int levelIndex = score.Key;
            String seconds = score.Value.ToString("F1") + "s\t\t" + "(" + "Radi: " + RADIS_BEST_TIMES[levelIndex] + "s)";

            sb.AppendLine("Level " + levelIndex + ": " + seconds);
        }

        completionTimes.text = sb.ToString();
    }
}
