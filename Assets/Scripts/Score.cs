using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{

    // Hacky global storage for Demo purposes
    public Dictionary<int, float> completionTimes = new Dictionary<int, float>();
    
    // Hacky global storage for Demo purposes
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
