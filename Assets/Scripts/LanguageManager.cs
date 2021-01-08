using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{

    public static LanguageManager Instance;

    
    public string languageSelected = "EN";
    void Awake()
    {
        if (Instance != null)
        {
            GameObject.Destroy(Instance);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("Language")!= null)
        {
           PlayerPrefs.GetString("Language", languageSelected);
        }
    }
    public void SelectLanguage(string languageSiglas)
    {
        languageSelected = languageSiglas;
        PlayerPrefs.SetString("Language", languageSelected);
    }
}

