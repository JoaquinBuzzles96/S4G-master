using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandPosition : MonoBehaviour
{
    public Transform handPos; //se usa para la tool excepto en el caso 7 que se utiliza para el telefono
    public Transform toolPos; //solo se usa en el caso 7 de momento pero podriamos utilizarlo en todos
    public Transform endoscopyHandPos;
    public TextMeshProUGUI canvasName;
}
