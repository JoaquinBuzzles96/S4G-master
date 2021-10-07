/*  ------ S4Game Consortium --------------
*  This library is free software; you can redistribute it and/or
*  modify it under the terms of the GNU Lesser General Public
*  License as published by the Free Software Foundation; either
*  version 3 of the License, or (at your option) any later version.
*  This library is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
*  Lesser General Public License for more details.
*  You should have received a copy of the GNU Lesser General Public
*  License along with this library; if not, write to the Free Software
*  CCMIJU, Carretera Nacional 521, Km 41.8 – 1007, Cáceres, Spain*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BlinkObject : MonoBehaviour
{
    public bool isActive;
    Color originalColor;
    public float speed;
    public float originalSpeed = 3.0f;

    public GameObject arrow;

    //Como este script solo se utilizara para la cuenta atras aprovechamos para modificar el color de la barra y de la cuenta atras
    public TextMeshProUGUI timerText;
    public Image countDownBar;

    void Start()
    {
        isActive = false;
        originalColor = arrow.gameObject.GetComponent<Renderer>().material.color;
        speed = originalSpeed;
    }


    void Update()
    {
        if (isActive)
        {
            // Arrow
            arrow.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(originalColor, Color.red, Mathf.PingPong(Time.time * speed, 1.0f));
            //Text
            timerText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * speed, 1.0f));
            //Bar
            countDownBar.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * speed, 1.0f));
        }
    }

    public void ResetValues()
    {
        arrow.gameObject.GetComponent<Renderer>().material.color = originalColor;
        speed = originalSpeed;
        isActive = false;
        countDownBar.color = Color.white;
        timerText.color = Color.white;
    }
}
