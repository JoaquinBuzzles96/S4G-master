using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsManager : MonoBehaviour
{

    public List<TextMeshProUGUI> options; //De momento las opciones estan seteadas en el editor, pero dejo esto aqui por si fuera necesario

    public TextMeshProUGUI selectedCase;

    public GameObject panelTutorialTransform;

    public GameObject triggerAreaTutorial;
    public GameObject teleportAreasTutorial;

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
            switch (selectedCase.text)
            {
                case "Tutorial":
                    //change panel position to 

                    this.gameObject.transform.position = panelTutorialTransform.transform.position;
                    this.gameObject.transform.rotation = panelTutorialTransform.transform.rotation;
                    EnableTutorial();

                    //SceneManager.LoadScene("Tutorial");
                    break;
                default:
                    LanguageManager.Instance.caseSelected = selectedCase.text;
                    SceneManager.LoadScene("Scene7");
                    break;
            }

        }
        
    }

    public void EnableTutorial()
    {
        triggerAreaTutorial.SetActive(true);
        teleportAreasTutorial.SetActive(true);
    }
}
