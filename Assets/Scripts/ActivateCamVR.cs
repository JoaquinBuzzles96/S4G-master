using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCamVR : MonoBehaviour
{
    public GameObject vr;
    public GameObject cameraNotVR;
    private void Awake()
    {
#if UNITY_EDITOR_WIN || UNITY_EDITOR 
        cameraNotVR.SetActive(true);
        vr.SetActive(false);
#endif

    }
}
