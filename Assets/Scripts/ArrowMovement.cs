using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    float maxMovement = 0.5f;
    float speed = 0.5f;
    float originalY;
    void Start()
    {
        originalY = transform.position.y;
    }


    void Update()
    {
        transform.position = new Vector3(transform.position.x, originalY + (Mathf.PingPong(Time.time * speed, maxMovement)), transform.position.z);
        //Mathf.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * speed, 1.0f));
    }
}
