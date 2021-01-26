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
        if (speed > 0)
        {
            //Debug.Log($"Estamos iluminando al {this.gameObject.name}");
        }

       text.color =  Color.Lerp(Color.white, Color.green, Mathf.PingPong(Time.time * speed, 1.0f));
    }
}

