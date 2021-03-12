using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    public float maxMovement = 2f;
    public float speed = 3f;
    float originalY;
    void Start()
    {
        originalY = transform.position.y;
    }


    void Update()
    {
        transform.position = new Vector3(transform.position.x, originalY + (Mathf.PingPong(speed, maxMovement)), transform.position.z);
        //Mathf.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * speed, 1.0f));
    }
}
