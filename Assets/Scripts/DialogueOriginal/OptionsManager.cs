using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    private static OptionsManager instance = null;

    // Game Instance Singleton
    public static OptionsManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public List<TextMeshProUGUI> options; //De momento las opciones estan seteadas en el editor, pero dejo esto aqui por si fuera necesario

    public TextMeshProUGUI selectedCase;

    public GameObject panelTutorialTransform;

    public Vector3 originPos;
    public Quaternion originRotation;

    public Vector3 secondPos;
    public Quaternion secondRotation;

    public GameObject triggerAreaTutorial;
    public GameObject teleportAreasTutorial;
    public GameObject TutorialPanel;

    public GameObject nameButton;
    public GameObject mailButton;

    void Start()
    {
        originPos = this.gameObject.transform.position;
        originRotation = this.gameObject.transform.rotation;

        secondPos = panelTutorialTransform.transform.position;
        secondRotation = panelTutorialTransform.transform.rotation;
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
                    this.gameObject.transform.position = secondPos;
                    this.gameObject.transform.rotation = secondRotation;
                    SetSettingsActive(false);
                    TutorialPanel.SetActive(true);
                    //SceneManager.LoadScene("Tutorial");
                    break;
                default:
                    LanguageManager.Instance.caseSelected = selectedCase.text;
                    SceneManager.LoadScene("Scene7");
                    break;
            }
        }
    }

    public void FinishTutorial()
    {
        EnableTutorial(true);
    }

    public void EnableTutorial(bool isActive)
    {
        triggerAreaTutorial.SetActive(isActive); //esto no esta tirando
        teleportAreasTutorial.SetActive(isActive);
    }

    public void SetSettingsActive(bool isActive)
    {
        this.nameButton.SetActive(isActive);
        this.mailButton.SetActive(isActive);
    }

    public void ResetScene() //este metodo tendra que llamarse desde el panel de la puntuacion del final de cada caso
    {
        SceneManager.LoadScene("Tutorial"); //cargamos la escena
        /*
         this.gameObject.transform.position = originPos; //asiganmos las posiciones iniciales
         this.gameObject.transform.rotation = originRotation;

         SetSettingsActive(true); //nombre y correo activos por si lo quieren cambiar
         EnableTutorial(false); //panel de las instrucciones
        */
        UI_Manager.Instance.Destroy();
        SpecialCases.Instance.Destroy();
        Destroy(this.gameObject);
        
    }
}
