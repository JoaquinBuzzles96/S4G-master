/*  ------ S4Game Consortium --------------
*  This library is free software; you can redistribute it and/or
*  modify it under the terms of the GNU Lesser General Public
*  License as published by the Free Software Foundation; either
*  version 3 of the License, or (at your option) any later version.
*  This library is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
*  Lesser General Public License for more details.
*  You should have received a copy of the GNU Lesser General Public
*  License along with this library; if not, write to the Free Software
*  CCMIJU, Carretera Nacional 521, Km 41.8 – 1007, Cáceres, Spain*/
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

    public bool isThirdPerson;
    public bool tutorialRay = false;
    void Start()
    {
        originPos = this.gameObject.transform.position;
        originRotation = this.gameObject.transform.rotation;

        secondPos = panelTutorialTransform.transform.position;
        secondRotation = panelTutorialTransform.transform.rotation;

        isThirdPerson = false;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void LoadLevel(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadLevelSelected()
    {
        Debug.Log($"Vamos a intentar cargar el caso {selectedCase.text} con el idioma {LanguageManager.Instance.languageSelected}");

        //TODO: Comprobar aqui que caso esta seleccionado y actualizar el idioma!!

        //Actualizamos el idioma
        LanguageManager.Instance.UpdateLanguage();


        if (IsInfoValid())
        {
            if (selectedCase.text != null && selectedCase.text != "")
            {
                string[] parts;
                parts = selectedCase.text.Split("_"[0]);

                for (int i = 0; i < parts.Length; i++)
                {
                    Debug.Log($"Hemos dividido el caso en {parts[i]}");
                }

                if (parts.Length > 1 && parts[1] == "ThirdPerson")
                {
                    Debug.Log($"Es tercera persona {parts[0]}");
                    LanguageManager.Instance.isThirdPerson = true;
                }
                else
                {
                    LanguageManager.Instance.isThirdPerson = false;
                }


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
                    /*
                case "CASE3_ThirdPerson":
                    selectedCase.text = "CASE3";
                    LanguageManager.Instance.caseSelected = selectedCase.text;
                    SceneManager.LoadScene("Scene7_thirdPerson");
                    break;
                    */
                    case "CASE5":
                        LanguageManager.Instance.caseSelected = parts[0];
                        SceneManager.LoadScene("Scene5");
                        break;
                    case "CASE6":
                        LanguageManager.Instance.caseSelected = parts[0];
                        SceneManager.LoadScene("Scene6");
                        break;
                    case "CASE7":
                        LanguageManager.Instance.caseSelected = parts[0];
                        SceneManager.LoadScene("Case7");
                        break;
                    default:
                        LanguageManager.Instance.caseSelected = parts[0];
                        SceneManager.LoadScene("Case3_9");


                        break;
                }
            }
        }
        else
        {
            Debug.Log("Se deben completar el campo del nombre y el correo para continuar");
        }
    }

    public bool IsInfoValid()
    {
        //No pedimos verificacion, si quisieramos hay que quitar el return
        return true; 
        //nos devolvera true si se han completado los camopos de name y correo
        if (SendMail.Instance.m_UserName == "Usuario Default" || SendMail.Instance.m_UserName == "" || SendMail.Instance.m_UserMail == "s4game@viralstudios.es" || SendMail.Instance.m_UserMail == "") 
        {
            return false;
        }

        return true;
    }

    public void FinishTutorial()
    {
        EnableTutorial(true);
    }

    public void EnableTutorial(bool isActive)
    {
        triggerAreaTutorial.SetActive(isActive); //esto no esta tirando
        teleportAreasTutorial.SetActive(isActive);
        tutorialRay = true;
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
        Debug.Log("Vamos a destruir el special cases ");
        SpecialCases.Instance.Destroy();
        Destroy(this.gameObject);

    }
}
