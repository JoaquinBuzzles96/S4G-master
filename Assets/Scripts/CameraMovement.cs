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
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    public float rotSpeed = 150.0f;
    public float movSpeed = 10.0f;
    public Vector3 initialPos;

    //Desplazamiento en los ejes X y Z
    float movX;
    float movZ;

    Vector3 mov;

    private void Start()
    {
        //this.transform.position = initialPos;
    }

    void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (OptionsManager.Instance.tutorialRay)
            {
                if (!IsPointerOverUIObject())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit))
                        {
                            print(hit.collider.name);
                            if (hit.collider.CompareTag("SueloClicable"))
                            transform.position = new Vector3(hit.point.x, this.transform.position.y, hit.point.z);
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                transform.eulerAngles += new Vector3(Input.GetAxisRaw("Mouse Y") * Time.deltaTime * rotSpeed, Input.GetAxisRaw("Mouse X") * Time.deltaTime * rotSpeed, 0.0f);
            }
        }
    }
    //When Touching UI
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
