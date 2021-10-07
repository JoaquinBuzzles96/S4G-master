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

public class ThrowObject : MonoBehaviour
{
    public GameObject objectToThrow;
    public GameObject objectiveToThrow;
    Rigidbody rb;
    Vector3 direction = Vector3.forward;
    public int force;

    private void Start()
    {
        rb = objectToThrow.GetComponent<Rigidbody>();
        direction = (objectiveToThrow.transform.position - this.transform.position).normalized;
    }
    public void ReleaseObject()
    {
        //Asignar el objeto que este en handPos en este momento
        objectToThrow = SpecialCases.Instance.GetProp(SpecialCases.Instance.currentTool);

        if (objectToThrow != null)
        {
            Debug.Log($"Vamos a lanzar la herramienta {SpecialCases.Instance.currentTool}");
            //Activamos el collider
            objectToThrow.GetComponent<BoxCollider>().enabled = true;
            objectToThrow.GetComponent<FollowPoint>().target = null;

            rb = objectToThrow.GetComponent<Rigidbody>();
            direction = (objectiveToThrow.transform.position - this.transform.position).normalized;


            //objectToThrow.transform.parent = null;

            rb.useGravity = true;
            rb.AddForce(direction * force);
        }
        else
        {
            Debug.Log("No tiene ningun objeto en la mano ahora");
        }


    }
}
