﻿/*  ------ S4Game Consortium --------------
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

public class SimpleWaypointMovement : MonoBehaviour
{
    Animator animator;
    public Transform path;
    public Transform[] waypoints;
    public Transform[] waypointsExit;
    public Transform[] waypointsEnter;
    int nextWaypoint = 0;
    public float speed;
    public bool canMove;
    public Transform lastPointToLook;
    public bool exitRoom = true;
    public bool isNurse = false;
    Transform[] wayToFollow;
    public bool firstTime = true;
    Transform originalPos;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        waypoints = path.GetComponentsInChildren<Transform>();
        GameObject gameObjectInitPoint = new GameObject("InitPos");
        var instance = Instantiate(gameObjectInitPoint, this.transform.position, this.transform.rotation);
        originalPos = instance.transform; //si le asigno directamente la transform hara un puntero e ira actualizando la posicion, y no queremos eso
        originalPos.position = new Vector3(originalPos.position.x, 0.36f, originalPos.position.z);
        wayToFollow = waypoints; //por si acaso se le llamase sin nada, pero siempre deberia indicarse antes de llamar
    }

    void Update()
    {
        //Debug.Log(SpecialCases.Instance.playingAnimation);

        if (canMove)
        {

            if (wayToFollow[nextWaypoint] != null)
            {
                if (Vector3.Distance(this.transform.position, wayToFollow[nextWaypoint].transform.position) > 0.1f)
                {
                    //Debug.Log("La "+ this.gameObject.name +"camina hacia el waypoint " + nextWaypoint);
                    if (!animator.GetBool("Walk"))
                    {
                        animator.SetBool("Walk", true);
                    }
                   // Vector3 vectorToGo = new Vector3(waypoints[nextWaypoint].position.x, this.gameObject.transform.position.y, waypoints[nextWaypoint].position.z);
                    this.transform.position = Vector3.MoveTowards(this.transform.position, wayToFollow[nextWaypoint].position, speed * Time.deltaTime);
                    this.transform.LookAt(wayToFollow[nextWaypoint]);
                }
                else if (nextWaypoint < wayToFollow.Length-1)
                {
                    //Debug.Log("cambio de "+nextWaypoint);
                    nextWaypoint++;
                }
                else
                {
                    //Debug.Log("acaba");
                    //SpecialCases.Instance.playingAnimation = false; //esto lo gestionaremos fuera, ya que llegar al destino no siemore significa que se acaba este evento
                    canMove = false;
                    nextWaypoint = 0;
                    this.transform.LookAt(lastPointToLook);
                    if (animator.GetBool("Walk"))
                    {
                        animator.SetBool("Walk", false);
                    }
                }
            }
        }
    }

    public void SetPathAndPlay(Transform[] _waypoints, Transform lookPoint)
    {
        //ClearWaypoints(); // No deberia ser necesario pero por si acaso
        //waypoints = _waypoints;
        lastPointToLook = lookPoint;
        wayToFollow = _waypoints;

        //Debug.Log("Por lo tanto el way to follow es: ");
        foreach (var item in wayToFollow)
        {
            //Debug.Log($"{item.position}");
        }

        canMove = true;
    }

    public void ResetPosition()
    {
        //ClearWaypoints();
        //waypoints[0] = originalPos;
        lastPointToLook = Case3Resources.Instance.endoscopist1LookPoint;
        Transform[] comeBackPoints = new Transform[1];
        comeBackPoints[0] = originalPos;
        wayToFollow = comeBackPoints;
        canMove = true;
    }

    public void ClearWaypoints()
    {
        System.Array.Clear(waypoints, 0, waypoints.Length);
    }
}
