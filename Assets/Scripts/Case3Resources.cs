using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case3Resources : MonoBehaviour
{
    private static Case3Resources instance = null;

    public static Case3Resources Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject secretary;
    public GameObject nurse;
    public Animation doorAnim;
    public GameObject fakeProp;
    public GameObject tablePoint;
    public GameObject floorPoint;

    public Transform[] waypointsExit;
    public Transform[] waypointsEnter;
    public Transform[] waypointsSecretary;
    public Transform[] waypointsToTable1;
    public Transform[] waypointsToTable2;

}
