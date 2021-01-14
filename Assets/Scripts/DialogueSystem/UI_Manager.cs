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
    private List<DialogueNodeData> situationDialogues = new List<DialogueNodeData>();
    private List<DialogueNodeData> answerDialogues = new List<DialogueNodeData>();

    public DialogueContainer dialogueContainer;

    

    //Screen 1
    public GameObject screen1;
    public TextMeshProUGUI contextDescription;

    //Screen 2
    public GameObject screen2;
    public List<TextMeshProUGUI> situationDialogueTexts = new List<TextMeshProUGUI>(); //Array de dialogos de situacion

    //Screen 3
    public GameObject screen3;

    //Screen 4
    public GameObject screen4;
    public List<TextMeshProUGUI> answerDialogueTexts = new List<TextMeshProUGUI>(); //Array de dialogos de situacion

    //Screen 5
    public GameObject screen5;
    public TextMeshProUGUI feedbackText;

    public AnswerNodeData choosenAnswer;
    public AudioSource audioSource;

    public List<Animator> characteres;

    public Dictionary<string, Animator> dictionaryCharacteres = new Dictionary<string, Animator>();

    public string caso;

    int lastQuestion;
    int currentQuestion;

    [HideInInspector]
    public string playereRoute;
    public float lastTime;
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
        //OLD
        /*
        //GetDialogueContainerLanguage(); //TODO: descomentar esto, esta comentado solo para probar casos de ejemplo
        SetNameToCharacters();
        lastQuestion = -1;
        SetupUI(dialogueContainer.GetFirstSituation());
        */

        //GetDialogueContainerLanguage(); //TODO: descomentar esto, esta comentado solo para probar casos de ejemplo
        SetNameToCharacters();
        lastQuestion = -1;
        SetUpContext(dialogueContainer.GetFirstSituation());
    }

    void Update()
    {
        
    }

    public void SetupUI(SituationNodeData _situation)
    {
        situation = _situation;
        
        if (LoadDialogues(situation.Guid, situationDialogues))
        {
            //situationDialogueTexts.text = dialogues[0].DialogueText;
            //Debug.Log($"Se ha asignado el primer dialogo de la situacion {situation.SituationName}, dialogo = {situationDialogueTexts.text}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la situacion {situation.SituationName}");
            //situationDialogueTexts.text = situation.Context;
        }
        
        questions = dialogueContainer.GetSituationQuestions(situation.Guid);

        AddTextToRoute(situation.SituationName);

        Debug.Log($"Se ha configurado la situacion {situation.SituationName}, tiene {questions.Count} preguntas posibles");
        //foreach (var item in questions){Debug.Log($"{item.QuestionName}");}
    }

    public bool LoadDialogues(string guid, List<DialogueNodeData> dialogues)
    {
        Debug.Log($"Vamos a comprobar si la situacion/respuesta actual tiene algun dialogo asignado");
        var firstElement = dialogueContainer.GetNextDialogueData(guid);
        Debug.Log($"Hemos obtenido el nodo {firstElement.DialogueName}");

        if (firstElement != null)
        {
            dialogues.Add(firstElement);//suponemos que siempre hay al menos uno
            Debug.Log($"Se ha añadido el dialogo {dialogues[dialogues.Count - 1].DialogueName}");
        }
        else
        {
            return false;
        }
        
        bool end = false;
        while (!end)
        {
            var aux = dialogueContainer.GetNextDialogueData(dialogues[dialogues.Count - 1].Guid);
            
            if (aux == null)
            {
                end = true;
            }
            else
            {
                dialogues.Add(aux);
                Debug.Log($"Se ha añadido el dialogo {aux.DialogueName}");
            }
        }

        Debug.Log("Ya no hay mas dialogos");
        return true;
    }

    public void ChangeStateUI(GameObject toDisable, GameObject toEnable)
    {
        toDisable.SetActive(false);
        toEnable.SetActive(true);
    }

    public void ToScreen1(SituationNodeData newSituation, GameObject originScreen) // Context
    {
        //OLD
        /*
        SetupUI(newSituation);
        screen1.SetActive(true);
        screen2.SetActive(false);
        */
        SetUpContext(newSituation);
        originScreen.SetActive(false);
        screen1.SetActive(true);

    }
    public void ToScreen2(GameObject originScreen) //Dialogue situation
    {
        //OLD
        /*
        screen2.SetActive(true);
        screen1.SetActive(false);
        OnStartButton();
        */


        SetUpScreen2(); //Actualizamos los dialogos de la pantalla
        originScreen.SetActive(false);
        screen2.SetActive(true);
    }

    public void ToScreen3(GameObject originScreen) // Answer choose
    {
        originScreen.SetActive(false);
        screen3.SetActive(true);
        SetUpScreen3();
    }

    public void ToScreen4(AnswerNodeData answer,GameObject originScreen) // Answer Dialogue
    {
        originScreen.SetActive(false);
        screen4.SetActive(true);
        SetUpScreen4(answer);
    }

    public void ToScreen5(AnswerNodeData answer, GameObject originScreen) //Feedback
    {
        originScreen.SetActive(false);
        screen5.SetActive(true);
        SetUpScreen5(answer);
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
            //Debug.Log($"Se ha añadido el {item.gameObject.name} al diccionario");
        }
    }

    public void AddTextToRoute(string _text)
    {
        playereRoute += "\n" + _text;
    }

    private void ReadContext()
    {
        if (isValid(situation.Context))
        {
            //TODO: poner en el panel correspondiente este texto
        }
    }

    public bool isValid(string texto)
    {
        if (texto == null || texto == "" || texto == "Deprecated field (use description node)")
        {
            Debug.LogError($"El texto utilizado no es valido");
            return false;
        }

        return true;
    }

    //New functions:

    public void CheckIfISEnd(GameObject originScreen)
    {
        //TODO: Vamos a la screen 2 o 5 en funcion del valor isEnd de la respuesta
        if (choosenAnswer.IsEnd)
        {
            ToScreen5(choosenAnswer, originScreen);
        }
        else
        {
            //Check if situation context no es nulo
            situation = dialogueContainer.GetNextSituation(choosenAnswer.Guid);

            if (isValid(situation.Context))
            {
                ToScreen1(situation, originScreen);
            }
            else
            {
                ToScreen2(originScreen);
            }
        }
    }

    public void SetUpContext(SituationNodeData _situation)
    {
        situation = _situation;

        contextDescription.text = situation.Context;

        Debug.Log($"Se ha configurado el contexto de la situacion {situation.SituationName}");
        /*
        if (LoadDialogues(situation.Guid, dialogues))
        {
            descriptionText.text = dialogues[0].DialogueText;
            Debug.Log($"Se ha asignado el primer dialogo de la situacion {situation.SituationName}, dialogo = {descriptionText.text}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la situacion {situation.SituationName}");
            descriptionText.text = situation.Context;
        }

        questions = dialogueContainer.GetSituationQuestions(situation.Guid);

        AddTextToRoute(situation.SituationName);

        Debug.Log($"Se ha configurado la situacion {situation.SituationName}, tiene {questions.Count} preguntas posibles");
        //foreach (var item in questions){Debug.Log($"{item.QuestionName}");}
        */
    }

    public void SetUpScreen2() //situation dialogues setUp
    {
        //Volvemos a pedir este parametro aunque en teoria ya lo tenemos, esto es para cubrir el caso en el que nesto sea invocado desde la screen 4
        //situation = _situation; //esto tiene que estar configurado ya
        if (LoadDialogues(situation.Guid, situationDialogues)) 
        {
            for (int i = 0; i < situationDialogues.Count; i++)
            {
                situationDialogueTexts[i % 4].text = situationDialogues[i].DialogueText;
            }

            Debug.Log($"Se ha asignado el primer dialogo de la situacion {situation.SituationName}, dialogo = {situationDialogues[0].DialogueText}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la situacion {situation.SituationName}");
        }
    }

    public void SetUpScreen3() //Asignar la pregunta y las 4 posibles respuestas
    {
        //En este caso si podemos suponer que la situacion que tenemos es la correcta, de modo que no es necesario pasarla por parametro
        questions = dialogueContainer.GetSituationQuestions(situation.Guid);

        currentQuestion = Random.Range(0, questions.Count);

        if (currentQuestion == lastQuestion)
        {
            currentQuestion++;
            currentQuestion = currentQuestion % questions.Count;
            //Debug.Log($"Como la pregunta era la misma que la anterior la cambiamos: current: {currentQuestion} last: {lastQuestion}");
        }
        //Debug.Log("Vamos a configurar el question data");

        screen3.GetComponent<QuestionUI>().questionData = questions[currentQuestion];
        screen3.GetComponent<QuestionUI>().SetupQuestion();

        lastQuestion = currentQuestion;

        Debug.Log($"Se ha configurado la screen 3 con la pregunta y las respuestas de la situacion {situation.SituationName}, pregunta {questions[currentQuestion].QuestionName}");
    }

    public void SetUpScreen4(AnswerNodeData answer)
    {
        choosenAnswer = answer;
        if (LoadDialogues(answer.Guid, answerDialogues))
        {
            //Suponemos que no hay mas de 4 dialogos (de momento)
            for (int i =0; i < answerDialogues.Count; i++)
            {
                answerDialogueTexts[i%4].text = answerDialogues[i].DialogueText;
            } 

            Debug.Log($"Se ha asignado el primer dialogo de la answer {answer.AnswerName}, dialogo = {answerDialogues[0].DialogueText}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la answer {answer.AnswerName}");
        }
    }

    public void SetUpScreen5(AnswerNodeData answer)
    {
        feedbackText.text = answer.Feedback;
    }

}
