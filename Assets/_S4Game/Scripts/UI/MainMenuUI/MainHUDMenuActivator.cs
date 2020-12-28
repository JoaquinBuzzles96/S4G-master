using Michsky.UI.ModernUIPack;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayMakerFSM))]
public class MainHUDMenuActivator : MonoBehaviour
{
  [Header("Related objects")]
  public GameObject mainHUDMenu;
  public GameObject centerEyeAnchor;
  public PlayMakerFSM fsmHudVisualization;

  private void Awake()
  {
    fsmHudVisualization = GetComponent<PlayMakerFSM>();

    //windowManager.
  }

  [Button()]
  public void ToogleMenu()
  {
    if (fsmHudVisualization.ActiveStateName == "ACTIVE")
    {
      DeactivateMenu();
    }
    else if (fsmHudVisualization.ActiveStateName == "HIDDEN")
    {
      ActivateMenu();
    }
  }

  [Button()]
  public void ActivateMenu()
  {
    fsmHudVisualization.SendEvent("ACTIVE");
  }

  [Button()]
  public void DeactivateMenu()
  {
    fsmHudVisualization.SendEvent("HIDDEN");
  }



  /// <summary>
  /// It shows the Main HUD Menu and aligns it with the camera (headset)
  /// </summary>
  [Button()]
  public void ShowMainHUDMenu()
  {

    #region HUD alignment with Heaset
    mainHUDMenu.transform.SetParent(centerEyeAnchor.transform);
    mainHUDMenu.transform.localPosition = new Vector3(0.0f, 0.0f, 0.85f);
    mainHUDMenu.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    mainHUDMenu.transform.SetParent(transform);
    Vector3 NewHUDRotation = new Vector3(mainHUDMenu.transform.localRotation.eulerAngles.x, mainHUDMenu.transform.localRotation.eulerAngles.y, 0.0f);
    mainHUDMenu.transform.localEulerAngles = NewHUDRotation;
    #endregion*/

  }
}
