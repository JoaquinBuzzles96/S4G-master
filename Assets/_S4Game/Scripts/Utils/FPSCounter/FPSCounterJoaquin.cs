using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounterJoaquin : MonoBehaviour
{
    [SerializeField]
    private Text FramesText;

    float refrehRate = 0.5f;
    float timer = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if (timer <= 0)
        {
            timer = refrehRate;
            FramesText.text = $"FPS: {1/Time.deltaTime}";
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
