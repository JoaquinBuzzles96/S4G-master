using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandAnimator : MonoBehaviour
{
    public float speed = 5.0f;
    public XRController controller = null;

    public CharacterController characterController;
    public GameObject parent;

    private float turnDegrees;
    private bool canTurn = true;

    bool isExitButtonActive = false;

    private Animator animator = null;
    private float cooldown;
    private float cooldownValue = 0.3f;

    private readonly List<Finger> gripFingers = new List<Finger>
    {
        new Finger(FingerType.Middle),
        new Finger(FingerType.Ring),
        new Finger(FingerType.Pinky)
    };

    private readonly List<Finger> pointFingers = new List<Finger>
    {
        new Finger(FingerType.Index),
        new Finger(FingerType.Thumb)
    };

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Store input
        CheckInputs();

        // Smooth input values
        SmoothFinger(pointFingers);
        SmoothFinger(gripFingers);

        // Apply smoothed values
        AnimateFinger(pointFingers);
        AnimateFinger(gripFingers);

        turnDegrees = 45;
    }

    private void CheckInputs()
    {
        controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
        SetFingerTargets(pointFingers, gripValue);

        controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryValue);
        controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue);

        if (primaryValue && secondaryValue)
        {
            SetFingerTargets(gripFingers, gripValue);
        }
        else if (primaryValue)
        {
            SetFingerTargets(gripFingers, 1f);
        }
        else if (secondaryValue)
        {
            SetFingerTargets(gripFingers, 1f);
        }
        else
        {
            SetFingerTargets(gripFingers, gripValue);
        }

        controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton);
        //Boton de salir
        if (gripButton && cooldown <= 0f)
        {
            Debug.Log("Se esta pulsando el boton de salir");

            isExitButtonActive = !isExitButtonActive;

            UI_Manager.Instance.ExitButton.SetActive(isExitButtonActive);
            cooldown = cooldownValue;
        }
        else
        {
            cooldown -= Time.deltaTime;
        }


        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightPrimaryAxisValue))
        {
            if (rightPrimaryAxisValue.x != 0 && canTurn)
            {
                if (rightPrimaryAxisValue.x > 0)
                {
                    parent.transform.Rotate(new Vector3(0, turnDegrees, 0));
                    canTurn = false;
                }
                else if (rightPrimaryAxisValue.x < 0)
                {
                    parent.transform.Rotate(new Vector3(0, -turnDegrees, 0));
                    canTurn = false;
                }
                /*
                Vector3 direction = new Vector3(rightPrimaryAxisValue.y, 0, rightPrimaryAxisValue.y); //TODO: cambiar esto a que sea solo un giro
                characterController.Move(direction);
                */
            }
            else if(rightPrimaryAxisValue.x == 0)
            {
                canTurn = true;
            }
        }

        

    }

    private void SetFingerTargets(List<Finger> fingers, float value)
    {
        foreach (Finger finger in fingers)
        {
            finger.target = value;
        }
    }

    private void SmoothFinger(List<Finger> fingers)
    {
        foreach (Finger finger in fingers)
        {
            float time = speed * Time.unscaledDeltaTime;
            finger.current = Mathf.MoveTowards(finger.current, finger.target, time);
        }
    }

    private void AnimateFinger(List<Finger> fingers)
    {
        foreach (Finger finger in fingers)
        {
            AnimateFinger(finger.type.ToString(), finger.current);
        }
    }

    private void AnimateFinger(string finger, float blend)
    {
        animator.SetFloat(finger, blend);
    }
}