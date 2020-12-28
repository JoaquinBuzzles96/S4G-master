using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[DefaultExecutionOrder(kControllerManagerUpdateOrder)]
public class InteractorsManager : MonoBehaviour
{
  // Slightly after the default, so that any actions such as release or grab can be processed *before* we switch controllers.
  public const int kControllerManagerUpdateOrder = 10;

  private InputDevice leftDevice;
  private InputDevice rightDevice;

  public InputHelpers.Button directActivationButton = InputHelpers.Button.Grip;
  public InputHelpers.Button teleportActivationButton = InputHelpers.Button.PrimaryButton;
  public InputHelpers.Button uiActivationButton = InputHelpers.Button.SecondaryButton;
  // Menu activation doesn't have exactly to do with interactors but with controllers, reason why its events are registered here
  public InputHelpers.Button menuButton = InputHelpers.Button.MenuButton;

  public GameObject leftDirectController;
  public GameObject leftTeleportController;
  public GameObject leftUIController;
  public GameObject rightDirectController;
  public GameObject rightTeleportController;
  public GameObject rightUIController;

  public UnityEvent onMenuButtonDown;
  private bool lastMenuButtonState = false;

  private struct InteractorController
  {
    public GameObject go;
    public XRController controller;
    public XRInteractorLineVisual lineRenderer;
    public XRBaseInteractor interactor;

    public void Attach(GameObject gameObject)
    {
      go = gameObject;
      if (go != null)
      {
        controller = go.GetComponent<XRController>();
        lineRenderer = go.GetComponent<XRInteractorLineVisual>();
        interactor = go.GetComponent<XRBaseInteractor>();

        Leave();
      }
    }

    public void Enter()
    {
      if (controller)
      {
        controller.enableInputActions = true;
      }
      if (lineRenderer)
      {
        lineRenderer.enabled = true;
      }
      if (interactor)
      {
        interactor.enabled = true;
      }
    }

    public void Leave()
    {
      if (controller)
      {
        controller.enableInputActions = false;
      }
      if (lineRenderer)
      {
        lineRenderer.enabled = false;
      }
      if (interactor)
      {
        interactor.enabled = false;
      }
    }
  }

  public enum ControllerStates
  {
    Direct = 0,
    Teleport = 1,
    UI = 2,
    MAX = 3
  }

  private struct ControllerState
  {
    private ControllerStates state;
    private InteractorController[] interactors;

    public void Initialize()
    {
      state = ControllerStates.MAX;
      interactors = new InteractorController[(int)ControllerStates.MAX];
    }

    public void ClearAll()
    {
      if (interactors == null)
      {
        return;
      }

      for (int i = 0; i < (int)ControllerStates.MAX; i++)
      {
        interactors[i].Leave();
      }
    }

    public void SetGameObject(ControllerStates state, GameObject parentGameObject)
    {
      if ((state == ControllerStates.MAX) || (interactors == null))
      {
        return;
      }

      interactors[(int)state].Attach(parentGameObject);
    }

    public void SetState(ControllerStates nextState)
    {
      if (nextState == state || nextState == ControllerStates.MAX)
      {
        return;
      }
      else
      {
        if (state != ControllerStates.MAX)
        {
          interactors[(int)state].Leave();
        }

        state = nextState;
        interactors[(int)state].Enter();
      }
    }
  }

  private ControllerState leftControllerState;
  private ControllerState rightControllerState;

  private void OnEnable()
  {
    leftControllerState.Initialize();
    rightControllerState.Initialize();

    leftControllerState.SetGameObject(ControllerStates.Direct, leftDirectController);
    leftControllerState.SetGameObject(ControllerStates.Teleport, leftTeleportController);
    leftControllerState.SetGameObject(ControllerStates.UI, leftUIController);

    rightControllerState.SetGameObject(ControllerStates.Direct, rightDirectController);
    rightControllerState.SetGameObject(ControllerStates.Teleport, rightTeleportController);
    rightControllerState.SetGameObject(ControllerStates.UI, rightUIController);

    leftControllerState.ClearAll();
    rightControllerState.ClearAll();

    InputDevices.deviceConnected += RegisterDevices;
    List<InputDevice> devices = new List<InputDevice>();
    InputDevices.GetDevices(devices);
    for (int i = 0; i < devices.Count; i++)
    {
      RegisterDevices(devices[i]);
    }
  }

  private void OnDisable()
  {
    InputDevices.deviceConnected -= RegisterDevices;
  }

  private void RegisterDevices(InputDevice connectedDevice)
  {
    if (connectedDevice.isValid)
    {
      if ((connectedDevice.characteristics & InputDeviceCharacteristics.Left) != 0)
      {
        leftDevice = connectedDevice;
      }
      else if ((connectedDevice.characteristics & InputDeviceCharacteristics.Right) != 0)
      {
        rightDevice = connectedDevice;
      }
    }
  }

  private void Update()
  {
    if (leftDevice.isValid)
    {
      leftDevice.IsPressed(directActivationButton, out bool direct);
      leftDevice.IsPressed(teleportActivationButton, out bool teleport);
      leftDevice.IsPressed(uiActivationButton, out bool ui);
      leftDevice.IsPressed(menuButton, out bool menu);


      if ((teleport && ui) || direct)
      {
        leftControllerState.SetState(ControllerStates.Direct);
      }
      else if (teleport)
      {
        leftControllerState.SetState(ControllerStates.Teleport);
      }
      else if (ui)
      {
        leftControllerState.SetState(ControllerStates.UI);
      }
      else
      {
        leftControllerState.SetState(ControllerStates.Direct);
      }

      if (menu && !lastMenuButtonState)
      {
        onMenuButtonDown.Invoke();
      }

      lastMenuButtonState = menu;
    }

    if (rightDevice.isValid)
    {
      rightDevice.IsPressed(directActivationButton, out bool direct);
      rightDevice.IsPressed(teleportActivationButton, out bool teleport);
      rightDevice.IsPressed(uiActivationButton, out bool ui);

      if ((teleport && ui) || direct)
      {
        rightControllerState.SetState(ControllerStates.Direct);
      }
      else if (teleport)
      {
        rightControllerState.SetState(ControllerStates.Teleport);
      }
      else if (ui)
      {
        rightControllerState.SetState(ControllerStates.UI);
      }
      else
      {
        rightControllerState.SetState(ControllerStates.Direct);
      }

    }
  }
}
