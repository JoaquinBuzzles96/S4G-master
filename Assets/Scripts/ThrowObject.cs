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
        objectToThrow.transform.parent = null;

        rb.useGravity = true;
        rb.AddForce(direction * force);
    }
}
