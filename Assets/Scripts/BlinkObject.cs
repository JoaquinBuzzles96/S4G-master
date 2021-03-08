using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkObject : MonoBehaviour
{
    public bool isActive;
    Color originalColor;
    public float speed;
    public float originalSpeed = 3.0f;

    void Start()
    {
        isActive = false;
        originalColor = this.gameObject.GetComponent<Renderer>().material.color;
        speed = originalSpeed;
    }


    void Update()
    {
        if (isActive)
        {
            this.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(originalColor, Color.red, Mathf.PingPong(Time.time * speed, 1.0f));
        }
    }

    public void ResetValues()
    {
        this.gameObject.GetComponent<Renderer>().material.color = originalColor;
        speed = originalSpeed;
        isActive = false;
    }
}
