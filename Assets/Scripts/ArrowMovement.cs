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
