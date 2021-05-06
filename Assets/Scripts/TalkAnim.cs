using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalkAnim : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Color colorBlink = Color.green;
    public float speed;
    public void Start()
    {
        //StartCoroutine(ToGreen());
    }

    public void Update()
    {
        if (speed > 0)
        {
            //Debug.Log($"Estamos iluminando al {this.gameObject.name}");
        }

       text.color =  Color.Lerp(Color.white, colorBlink, Mathf.PingPong(Time.time * speed, 1.0f));
    }
}

