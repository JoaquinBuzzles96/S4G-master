using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsManager : MonoBehaviour
{

    public List<TextMeshProUGUI> options; //De momento las opciones estan seteadas en el editor, pero dejo esto aqui por si fuera necesario

    public TextMeshProUGUI selectedCase;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LoadLevel(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadLevelSelected()
    {
        Debug.Log($"Vamos a intentar cargar el caso {selectedCase.text}");

        if (selectedCase.text != null && selectedCase.text != "")
        {
            LanguageManager.Instance.caseSelected = selectedCase.text;
            SceneManager.LoadScene("Scene7");
        }
        
    }
}
