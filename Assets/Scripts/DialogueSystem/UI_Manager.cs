using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager instance = null;

    // Game Instance Singleton
    public static UI_Manager Instance
    {
        get
        {
            return instance;
        }
    }

    private SituationNodeData situation;
    private List<QuestionNodeData> questions;

    public DialogueContainer dialogueContainer;

    public TextMeshProUGUI descriptionText;
    public GameObject screen1;
    public GameObject screen2;
    public AudioSource audioSource;

    public List<Animator> characteres;

    public Dictionary<string, Animator> dictionaryCharacteres = new Dictionary<string, Animator>();

    public string caso;

    int lastQuestion;
    int currentQuestion;

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
    void Start()
    {
        GetDialogueContainerLanguage();
        SetNameToCharacters();
        lastQuestion = -1;
        SetupUI(dialogueContainer.GetFirstSituation());

    }

    void Update()
    {
        
    }

    public void SetupUI(SituationNodeData _situation)
    {
        situation = _situation;
        descriptionText.text = situation.Description;
        questions = dialogueContainer.GetSituationQuestions(situation.Guid);

        Debug.Log($"Se ha configurado la situacion {situation.SituationName}, tiene {questions.Count} preguntas posibles");
    }

    public void ChangeStateUI(GameObject toDisable, GameObject toEnable)
    {
        toDisable.SetActive(false);
        toEnable.SetActive(true);
    }

    public void ToScreen1(SituationNodeData newSituation)
    {
        SetupUI(newSituation);
        screen1.SetActive(true);
        screen2.SetActive(false);
    }
    public void ToScreen2()
    {
        screen2.SetActive(true);
        screen1.SetActive(false);
        OnStartButton();
    }

    public void OnStartButton()
    {

        currentQuestion = Random.Range(0, questions.Count);

        if (currentQuestion == lastQuestion)
        {
            currentQuestion++;
            currentQuestion = currentQuestion % questions.Count;
            //Debug.Log($"Como la pregunta era la misma que la anterior la cambiamos: current: {currentQuestion} last: {lastQuestion}");
        }
        //Debug.Log("Vamos a configurar el question data");

        screen2.GetComponent<QuestionUI>().questionData = questions[currentQuestion];
        screen2.GetComponent<QuestionUI>().SetupQuestion();
        lastQuestion = currentQuestion;
    }

    public void PlayAudioOnSpeaker(string _audio, string _speaker)
    {
        //Animacion
        if (dictionaryCharacteres.ContainsKey(_speaker))
        {
            dictionaryCharacteres[_speaker].SetBool("isTalking", true);
        }
        else
        {
            Debug.Log($"No se ha encontrado el speaker {_speaker}");
        }

        //SetAnimToFalse(_speaker);

        //Ahora mismo da igual quien lo diga
        //Debug.Log("Se va a reproducir un audio");
        audioSource.clip = Resources.Load($"Audio/{caso}_{LanguageManager.Instance.languageSelected}/{_audio}", typeof(AudioClip)) as AudioClip;
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.Log($"No se ha encontrado el audio Audio/{caso}_{LanguageManager.Instance.languageSelected}/{_audio}");
        }

    }

    IEnumerator SetAnimToFalse(string _speaker)
    {
        yield return null;
        dictionaryCharacteres[_speaker].SetBool("isTalking", false);
    }

    public void GetDialogueContainerLanguage()
    {
        dialogueContainer = Resources.Load($"Cases/{caso}_{LanguageManager.Instance.languageSelected}") as DialogueContainer;
        if (dialogueContainer == null)
        {
            Debug.Log($"No se ha encontrado la ruta Cases/{caso}_{LanguageManager.Instance.languageSelected}");
            dialogueContainer = Resources.Load($"Cases/Case5_EN") as DialogueContainer;
        }
    }

    public void SetNameToCharacters()
    {
        foreach (var item in characteres)
        {
            dictionaryCharacteres.Add(item.gameObject.name, item);
            Debug.Log($"Se ha añadido el {item.gameObject.name} al diccionario");
        }
    }

}
