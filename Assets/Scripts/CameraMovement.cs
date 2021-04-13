using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public float speed = 25.0f;
	public Vector3 initialPos;


	private void Start()
    {
		//this.transform.position = initialPos;
    }
    void Update()
	{
		if (Input.GetMouseButton(1))
		{
			if (Input.GetAxis("Mouse X") != 0)
			{
				transform.eulerAngles += new Vector3(Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed, Input.GetAxisRaw("Mouse X") * Time.deltaTime * -speed, 0.0f);
			}
		}
	}
}
