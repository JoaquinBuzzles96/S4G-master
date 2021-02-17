using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(FPSCounter))]
public class FPSCounterLabel : MonoBehaviour
{
  [SerializeField]
  private FPSCounter fpsCounter;
  [SerializeField]
  private TextMeshProUGUI txtFPSCounter;

  private void Awake()
  {
    fpsCounter = GetComponent<FPSCounter>();
    fpsCounter.onFPSUpdated.AddListener(UpdateLabel);
  }

  private void UpdateLabel(float count)
  {
    if (txtFPSCounter)
    {
      txtFPSCounter.text = "FPS:" + (Mathf.Round(count));
    }

  }
}
