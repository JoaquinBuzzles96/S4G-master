using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadCases : MonoBehaviour
{
    List<DialogueContainer> cases = new List<DialogueContainer>();

    //public Text TextBox;

    void Start()
    {
        var dropdown = transform.GetComponent<TMP_Dropdown>();

        dropdown.options.Clear();

        InitialLoad();

        foreach (var item in cases)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = item.name});
        }

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    private void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        //caso a cargar = dropdown.value; //esto tiene el nombre
        Debug.Log($"Vamos a cargar el caso {dropdown.value}");
    }

    void Update()
    {
        
    }

    public void InitialLoad()
    {
        var casesArray = Resources.LoadAll("Cases", typeof(DialogueContainer));
        Debug.Log("Se han obtenido los siguientes casos:");
        foreach (var item in casesArray)
        {
            //Debug.Log($"{item.name}");
            cases.Add(item as DialogueContainer);
            //Debug.Log($"{cases[cases.Count-1].name}");
        }
    }
}
