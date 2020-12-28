using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickUIButton : MonoBehaviour
{
    private Button button;

    private void OnEnable()
    {
        button = GetComponent<Button>();
    }

    [ContextMenu("Click")]
    public void Click()
    {
        button.onClick.Invoke();
    }
}
