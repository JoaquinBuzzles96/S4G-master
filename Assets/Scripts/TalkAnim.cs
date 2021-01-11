using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalkAnim : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed;
    public void Start()
    {
        //StartCoroutine(ToGreen());
    }

    public void Update()
    {
       text.color =  Color.Lerp(Color.white, Color.green, Mathf.PingPong(Time.time * speed, 1.0f));
    }
}

