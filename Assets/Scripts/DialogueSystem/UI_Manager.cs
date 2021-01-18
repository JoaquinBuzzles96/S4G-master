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
    private int currentDialogue = 0; //podemos usar la misma variable para ambos casos ya que nunca se daran al mismo tiempo

    public DialogueContainer dialogueContainer;

    //Screen 1
    public GameObject screen1;
    public TextMeshProUGUI contextDescription;

    //Screen 2
    public GameObject screen2;
    public List<TextMeshProUGUI> situationDialogueTexts = new List<TextMeshProUGUI>();  //Array de dialogos de situacion
    public GameObject nextButtonSituation;

    //Screen 3
    public GameObject screen3;

    //Screen 4
    public GameObject screen4;
    public List<TextMeshProUGUI> answerDialogueTexts = new List<TextMeshProUGUI>(); //Array de dialogos de answer
    public GameObject nextButtonAnswer;

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
    public int totalScore; 
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
        GetDialogueContainerLanguage();
        SetNameToCharacters();
        lastQuestion = -1;
        totalScore = 0;
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

        //Debug.Log($"Se ha configurado la situacion {situation.SituationName}, tiene {questions.Count} preguntas posibles");
        //foreach (var item in questions){Debug.Log($"{item.QuestionName}");}
    }

    public bool LoadDialogues(string guid, List<DialogueNodeData> dialogues)
    {
        //Lo primero es vaciar los dialogos en caso de que ya se hubieran consultado antes
        dialogues.Clear();
        //Debug.Log($"Vamos a comprobar si la situacion/respuesta actual tiene algun dialogo asignado");
        var firstElement = dialogueContainer.GetNextDialogueData(guid);
        //Debug.Log($"Hemos obtenido el nodo {firstElement.DialogueName}");

        if (firstElement != null)
        {
            dialogues.Add(firstElement);//suponemos que siempre hay al menos uno
            //Debug.Log($"Se ha añadido el dialogo {dialogues[dialogues.Count - 1].DialogueName}");
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
                //Debug.Log($"Se ha añadido el dialogo {aux.DialogueName}");
            }
        }

        //Debug.Log("Ya no hay mas dialogos");
        return true;
    }

    public void ChangeStateUI(GameObject toDisable, GameObject toEnable)
    {
        toDisable.SetActive(false);
        toEnable.SetActive(true);
    }

    public void ToScreen1(SituationNodeData newSituation, GameObject originScreen) // Context
    {
        SetUpContext(newSituation);
        originScreen.SetActive(false);
        screen1.SetActive(true);

    }
    public void ToScreen2(GameObject originScreen) //Dialogue situation
    {
        SetUpScreen2(); //Actualizamos los dialogos de la pantalla
        originScreen.SetActive(false);
        screen2.SetActive(true);
    }

    public void ToScreen3(GameObject originScreen) // Answer choose
    {
        if (!CheckMoreDialogues(situationDialogues, situationDialogueTexts, nextButtonSituation)) //si no hay mas dialogos entra aqui
        {
            originScreen.SetActive(false);
            screen3.SetActive(true);
            SetUpScreen3();
        }
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

    public float PlayAudioOnSpeaker(string _audio, string _speaker, string _mood)
    {
        //Animacion
        //Debug.Log($"Comprobamos si el diccionario contiene {_speaker}");
        if (dictionaryCharacteres.ContainsKey(_speaker) && _speaker != "Narrator")
        {
            SetMoodAnim(_speaker, _mood);
        }
        else
        {
            Debug.Log($"No se ha encontrado el speaker {_speaker}");
        }

        //Audio
        audioSource.clip = Resources.Load(GetAudioPath(_audio), typeof(AudioClip)) as AudioClip;
        if (audioSource.clip != null)
        {
            audioSource.Play();
            //Debug.Log($"Hacemos una pausa de {audioSource.clip.length + 2.5f}");
            return audioSource.clip.length + 2.5f;
        }
        else
        {
            Debug.Log($"No se ha encontrado el audio Audio/{caso}_{LanguageManager.Instance.languageSelected}/{_audio}");
            return 0f;
        }

        
    }

    private void SetMoodAnim(string _speaker, string _mood)
    {
        
        switch (_mood)
        {
            case "Ironic":
                dictionaryCharacteres[_speaker].SetBool("isIronic", true);
                break;
            case "Regretful":
                dictionaryCharacteres[_speaker].SetBool("isRegret", true);
                break;
            case "Agressive":
                dictionaryCharacteres[_speaker].SetBool("isYelling", true);
                break;
            case "Nervous": //tambien hacen la default de momento
            case "Calmly":
            default:
                dictionaryCharacteres[_speaker].SetBool("isTalking", true); //por defecto hacemos la animacion generica
                break;
        }

    }

    private string GetAudioPath(string _audio)
    {
        string path = $"Audio/Case5_EN/{_audio}";
        if (isValid(caso) && LanguageManager.Instance != null)
        {
            if (isValid(LanguageManager.Instance.languageSelected))
            {
                path = $"Audio/{caso}_{LanguageManager.Instance.languageSelected}/{_audio}";
            }
        }

        return path;
    }

    public void GetDialogueContainerLanguage()
    {
        if (LanguageManager.Instance != null)
        {
            dialogueContainer = Resources.Load($"Cases/{caso}_{LanguageManager.Instance.languageSelected}") as DialogueContainer;
        }
        else
        {
            Debug.Log($"No se ha encontrado el languague manager");
            dialogueContainer = Resources.Load($"Cases/testing2") as DialogueContainer;
        }
        
        if (dialogueContainer == null)
        {
            Debug.Log($"No se ha encontrado la ruta Cases/{caso}_{LanguageManager.Instance.languageSelected}");
            dialogueContainer = Resources.Load($"Cases/testing2") as DialogueContainer;
        }
    }

    public void SetNameToCharacters()
    {
        foreach (var item in characteres)
        {
            dictionaryCharacteres.Add(item.gameObject.name, item);
            //Debug.Log($"Se ha añadido el {item.gameObject.name} al diccionario");
        }

        /* Case 3:
        Narrator
        Endoscopist 1
        Endoscopy Nurse
        Anaesthesiologist
        Endoscopist 2
        Secretary
         */
    }

    public void AddTextToRoute(string _text)
    {
        playereRoute += "\n" + _text;
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

    public void CheckIfISEnd(GameObject originScreen)
    {
        if (!CheckMoreDialogues(answerDialogues, answerDialogueTexts, nextButtonAnswer)) //si no hay mas dialogos entra aqui
        {
            //Vamos a la screen 2 o 5 en funcion del valor isEnd de la respuesta
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

    }

    public void SetUpContext(SituationNodeData _situation)
    {
        situation = _situation;

        contextDescription.text = situation.Context;

        Debug.Log($"Se ha configurado el contexto de la situacion {situation.SituationName}");

    }

    public void SetUpScreen2() //situation dialogues setUp
    {
        //situation = _situation; //esto tiene que estar configurado ya
        AddTextToRoute($"{situation.SituationName}: {situation.Context}"); //LO GUARDAMOS PARA EL CORREO

        if (LoadDialogues(situation.Guid, situationDialogues)) 
        {
            //RefreshDialogues(situationDialogues, situationDialogueTexts);
            StartCoroutine(PlayDialogues(situationDialogues, situationDialogueTexts, nextButtonSituation));

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

            //RefreshDialogues(answerDialogues, answerDialogueTexts);
            StartCoroutine(PlayDialogues(answerDialogues, answerDialogueTexts, nextButtonAnswer));

            Debug.Log($"Se ha asignado el primer dialogo de la answer {answer.AnswerName}, dialogo = {answerDialogues[0].DialogueText}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la answer {answer.AnswerName}");
        }
    }

    public void SetUpScreen5(AnswerNodeData answer)
    {
        feedbackText.text = $"{answer.Feedback} /n Score: {totalScore}";
        //Add feedback to email
        AddTextToRoute("\n Total time playing: " + Mathf.RoundToInt(Time.time / 60) + " minuts and " + Mathf.RoundToInt(Time.time % 60) + " seconds.");
        AddTextToRoute($"Feedback: {answer.Feedback}");
        AddTextToRoute($"Final score: {totalScore}");
        //Send email
        Debug.Log("Enviamos un mail con la infomacion");
        SendMail.Instance.SendEmail();
    }

    private bool CheckMoreDialogues(List<DialogueNodeData> dialoguesData, List<TextMeshProUGUI> dialoguesUI, GameObject nextButton)
    {
        bool refresh = false;
        if (currentDialogue < dialoguesData.Count) //esto significaria que quedan dialogos sin mostrar
        {
            Clear(dialoguesUI);
            //en este caso hay que hacer un refresh de la UI
            refresh = true;
            StartCoroutine(PlayDialogues(dialoguesData, dialoguesUI, nextButton));
        }
        else
        {
            Clear(dialoguesUI);
            currentDialogue = 0; //reseteamos para el siguiente
        }

        return refresh;
    }

    private void Clear(List<TextMeshProUGUI> dialoguesUI)
    {
        for (int i = 0; i < dialoguesUI.Count; i++)
        {
            dialoguesUI[i].text = "-";
        }
    }

    IEnumerator PlayDialogues(List<DialogueNodeData> dialoguesData, List<TextMeshProUGUI> dialoguesUI, GameObject nextButton)
    {
        nextButton.SetActive(false); // lo desactivamos mientras no haya que avanzar de pantalla
        bool end = false;
        int i = 0;
        //Debug.Log($"Entramos y current: {currentDialogue}, i: {i}");
        for (i = 0; i < dialoguesUI.Count && !end; i++)
        {
            if ((currentDialogue + i) < dialoguesData.Count)
            {
                dialoguesUI[i].text = $"{dialoguesData[currentDialogue + i].Speaker} ({dialoguesData[currentDialogue + i].Mood}): {dialoguesData[currentDialogue + i].DialogueText}";
                AddTextToRoute($"{dialoguesData[currentDialogue + i].Speaker} ({dialoguesData[currentDialogue + i].Mood}): {dialoguesData[currentDialogue + i].DialogueText}");
                //Debug.Log($"Hemos puesto el dialogo {currentDialogue + i}, ahora vamos a hacer una pausa");
                yield return new WaitForSeconds(PlayAudioOnSpeaker(dialoguesData[currentDialogue + i].audioId, dialoguesData[currentDialogue + i].Speaker, dialoguesData[currentDialogue + i].Mood));
            }
            else
            {
                end = true;
            }
        }

        currentDialogue += i;
        nextButton.SetActive(true);
    }

}
