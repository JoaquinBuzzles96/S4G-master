using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{

    public static LanguageManager Instance;

    
    public string languageSelected = "EN";
    public string caseSelected = "Case3";

    public bool isThirdPerson;
    void Awake()
    {
        if (Instance != null)
        {
            GameObject.Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("Language")!= null)
        {
            languageSelected = PlayerPrefs.GetString("Language");
        }

        isThirdPerson = false;
    }
    public void SelectLanguage(string languageSiglas)
    {
        languageSelected = languageSiglas;
        PlayerPrefs.SetString("Language", languageSelected);
    }


}

