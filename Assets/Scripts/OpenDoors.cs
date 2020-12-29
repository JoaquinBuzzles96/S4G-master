using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    [SerializeField]
    private Transform targetStance;
    [SerializeField]
    private Transform targetTransform;
    [SerializeField]
    private bool positionStance;
    [SerializeField]
    private float velocityForPosition;
    [SerializeField]
    private bool rotationStance;
    [SerializeField]
    private float velocityForRotation;

    private bool initiated;

    private bool positionFinalized;
    private bool rotationFinalized;

    private Vector3 currentPosition;
    private Vector3 currentRotation;

    public bool Finished { get; set; }

    protected void Start()
    {
        initiated = false;
        positionFinalized = false;
        rotationFinalized = false;
        currentPosition = Vector3.zero;
        currentRotation = Vector3.zero;
    }

    public void StartEvent()
    {
        initiated = true;
        positionFinalized = false;
        rotationFinalized = false;
    }

    private void Update()
    {
        if (!Finished && initiated && targetTransform)
        {
            if (positionStance && !positionFinalized)
            {
                targetTransform.position = Vector3.SmoothDamp(targetTransform.position, transform.position, ref currentPosition, velocityForPosition);
                if (Vector3.Distance(targetTransform.position, transform.position) < 0.1f)
                {
                    positionFinalized = true;
                }
            }

            if (rotationStance && !rotationFinalized)
            {
                targetTransform.forward = Vector3.SmoothDamp(targetTransform.forward, transform.forward, ref currentRotation, velocityForRotation);
                if (Vector3.Angle(targetTransform.forward, transform.forward) < 3f)
                {
                    rotationFinalized = true;
                }
            }

            if ((positionStance && positionFinalized) && !rotationStance)
            {
                Finished = true;
            }

            else if (!positionStance && (rotationStance && rotationFinalized))
            {
                Finished = true;
            }

            else if ((positionStance && positionFinalized) && (rotationStance && rotationFinalized))
            {
                Finished = true;
            }
        }
    }
    /*
    [SerializeField]
    private Transform targetTransform;

    private float currentRotation;
    private float speedRotation;
    private float targetRotation;
    private bool rotationFinalized;

    private bool initiated;
    public bool Finished { get; set; }

    protected void Start()
    {
        initiated = false;
        Finished = false;
        targetRotation = 90;
        speedRotation = 30;
    }

    public void StartEvent()
    {
        initiated = true;
    }

    private void Update()
    {
        if (!Finished && initiated)
        {
            Debug.Log("Se cumplen las condiciones, vamos a abrirlo");

            targetTransform.Rotate(Vector3.up * speedRotation * Time.deltaTime);

            Debug.Log("Rotation Y = " + targetTransform.rotation.y);
            Debug.Log("Rotation Y = " + targetTransform.rotation.y);

            if (targetTransform.rotation.y * Mathf.Rad2Deg >= targetRotation )
            {
                Debug.Log("Acaba el giro");
                Finished = true;
            }
        }
    }

    */
}
