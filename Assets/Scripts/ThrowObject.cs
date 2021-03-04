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
