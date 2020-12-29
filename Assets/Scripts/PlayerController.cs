using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    InputDevice rightController;
    InputDevice leftController;

    [SerializeField]
    private GameObject playerCamera;
    private float turnDegrees;

    void Start()
    {
        turnDegrees = 45;
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics desiredDevide = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredDevide, devices); //Con esto obtenemos todos los inputs de las Quest

        if (devices.Count > 0)
        {
            rightController = devices[0];
            Debug.Log($"Name: {rightController.name} -- Characteristics: {rightController.characteristics}");
        }
        else
        {
            Debug.Log("No se han encontrado dispositivos para la derecha");
        }


        desiredDevide = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredDevide, devices); //Con esto obtenemos todos los inputs de las Quest

        if (devices.Count > 0)
        {
            leftController = devices[0];
            Debug.Log($"Name: {leftController.name} -- Characteristics: {leftController.characteristics}");
        }
        else
        {
            Debug.Log("No se han encontrado dispositivos para la izquierda");
        }
    }

    
    void Update()
    {
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightPrimaryAxisValue) && rightPrimaryAxisValue != Vector2.zero)
        {
            Debug.Log("Estamos preparados para detectar movimiento");
            if (rightPrimaryAxisValue.x > 0)
            {
                Debug.Log("Giramos a la derecha");
                //mover camara a la der
                playerCamera.transform.rotation = new Quaternion(playerCamera.transform.rotation.x, playerCamera.transform.rotation.y + turnDegrees, playerCamera.transform.rotation.z, playerCamera.transform.rotation.w);
            }
            else if (rightPrimaryAxisValue.x < 0)
            {
                //mover camara a la izq
                playerCamera.transform.rotation = new Quaternion(playerCamera.transform.rotation.x, playerCamera.transform.rotation.y - turnDegrees, playerCamera.transform.rotation.z, playerCamera.transform.rotation.w);
            }
        }
        else if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftPrimaryAxisValue) && leftPrimaryAxisValue != Vector2.zero)
        {
            if (leftPrimaryAxisValue.x > 0)
            {
                //mover camara a la der
            }
            else if (leftPrimaryAxisValue.x < 0)
            {
                //mover camara a la izq
            }
        }
    }


}
