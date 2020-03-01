using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LevelTimer : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Time.timeSinceLevelLoad.ToString("F1") + "s";
    }
}
