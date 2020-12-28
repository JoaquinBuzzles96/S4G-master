using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerEvent : UnityEvent<GameObject> { }

public class TriggerArea : MonoBehaviour
{
    public List<GameObject> triggersGameObjects = new List<GameObject>();
    public TriggerEvent OnEnter = new TriggerEvent();
    public TriggerEvent OnStay = new TriggerEvent();
    public TriggerEvent OnExit = new TriggerEvent();

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if(triggersGameObjects.Contains(go))
        {
            OnEnter.Invoke(go);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject go = other.gameObject;
        if (triggersGameObjects.Contains(go))
        {
            OnStay.Invoke(go);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject go = other.gameObject;
        if (triggersGameObjects.Contains(go))
        {
            OnExit.Invoke(go);
        }
    }
}
