using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

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
    public GameObject nextButtonContext;

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
    public GameObject nextButtonFeedback;

    public AnswerNodeData choosenAnswer;
    public AudioSource audioSource;

    public List<Animator> characteres;

    public Dictionary<string, Animator> dictionaryCharacteres = new Dictionary<string, Animator>();

    public string caso;

    int lastQuestion;
    int currentQuestion;

    //RandomAnims
    float animTimer = 2.0f;
    float timer = 0f;
    int numOfExtraAnims = 2;

    //temporal:
    public TalkAnim endoescopista1;
    public TalkAnim endoescopista2;
    public TalkAnim anestesiologo;
    public TalkAnim enfermeraDeEndoscopia;
    public TalkAnim enfermeraDeAnestesia;



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
        CheckRandomAnims();
    }

    public void CheckRandomAnims()
    {
        //Cada x tiempo metemos una animacion random en un personaje random
        if (timer >= animTimer)
        {
            timer = 0f;
            string randomCharacter = characteres[Random.Range(0, characteres.Count)].gameObject.name;
            dictionaryCharacteres[randomCharacter].SetInteger("randomAnim", Random.Range(1, numOfExtraAnims + 1));
        }
        else
        {
            timer += Time.deltaTime;
        }
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
        string speaker = Translate(_speaker); //spanish to english
        //Comprobamos el audio de las enfermeras
        _audio = SpecialCases.Instance.ChechkAudio(_audio, speaker);

        //Animacion
        //Debug.Log($"Comprobamos si el diccionario contiene {_speaker}");
        if (dictionaryCharacteres.ContainsKey(speaker) && speaker != "Narrator")
        {
            SetMoodAnim(speaker, _mood);
            SetColorName(0.75f, speaker);
        }
        else
        {
            if (_speaker != "Narrador" && _speaker != "Narrator")
            {
                Debug.Log($"No se ha encontrado el speaker {_speaker} --> {speaker}");
            }
                
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
        //Ironic, Regretful, Agressive, Nervous. Calmly
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

    public string GetAudioPath(string _audio)
    {
        //string path = $"Audio/Case5_EN/{_audio}";
        string path = $"Audio/Case5_EN/Audio1"; //default value
        if (isValid(caso) && LanguageManager.Instance != null && isValid(_audio))
        {
            if (isValid(LanguageManager.Instance.languageSelected))
            {
                path = $"Audio/{caso}_{LanguageManager.Instance.languageSelected}/{_audio}";
            }
            else
            {
                Debug.Log("No se ha encontrado el audio por lo que asignamos uno por defecto");
                path = $"Audio/Case3_ES/Audio1";
            }
        }
        else
        {
            Debug.Log("No se ha encontrado el audio por lo que asignamos uno por defecto");
            path = $"Audio/Case3_ES/Audio1";
        }

        //Debug.Log($"Vamos a devolver el path {path}");
        
        return path;
    }

    public void GetDialogueContainerLanguage()
    {
        string path = "";
        //Application.streamingAssetsPath + "/Resources/Audios/";
        /*
        #if UNITY_EDITOR 
        if (LanguageManager.Instance != null)
        {
            caso = LanguageManager.Instance.caseSelected; //TODO: En un futuro funcionara con esto, de momento dejarlo comentado
            path = $"Cases/{caso}_{LanguageManager.Instance.languageSelected}";
            //dialogueContainer = Resources.Load($"Cases/{caso}_{LanguageManager.Instance.languageSelected}") as DialogueContainer;
            Debug.Log($"Cargamos el case Cases/{caso}_{LanguageManager.Instance.languageSelected}");
        }
        else
        {
            path = $"Cases/testing2";
            Debug.Log($"No se ha encontrado el languague manager");
            //dialogueContainer = Resources.Load($"Cases/testing2") as DialogueContainer;
        }

        if (dialogueContainer == null)
        {
            path = $"Cases/testing2";
            Debug.Log($"No se ha encontrado la ruta Cases/{caso}_{LanguageManager.Instance.languageSelected}");
            //dialogueContainer = Resources.Load($"Cases/testing2") as DialogueContainer;
        }
        #endif

        */
        

         

        //#if !UNITY_EDITOR
        if (LanguageManager.Instance != null)
        {
            caso = LanguageManager.Instance.caseSelected; //TODO: En un futuro funcionara con esto, de momento dejarlo comentado
            path = Application.streamingAssetsPath + $"/Resources/Cases/{caso}_{LanguageManager.Instance.languageSelected}";
            //dialogueContainer = Resources.Load($"Cases/{caso}_{LanguageManager.Instance.languageSelected}") as DialogueContainer;
            //Debug.Log($"Cargamos el case Cases/{caso}_{LanguageManager.Instance.languageSelected}");
        }
        else
        {
            path = Application.streamingAssetsPath + $"/Resources/Cases/testing2";
            //Debug.Log($"No se ha encontrado el languague manager");
            //dialogueContainer = Resources.Load($"Cases/testing2") as DialogueContainer;
        }

        if (dialogueContainer == null)
        {
            path = Application.streamingAssetsPath + $"/Resources/Cases/testing2";
            //Debug.Log($"No se ha encontrado la ruta Cases/{caso}_{LanguageManager.Instance.languageSelected}");
            //dialogueContainer = Resources.Load($"Cases/testing2") as DialogueContainer;
        }
        //#endif

        var casesArray = Resources.LoadAll("Cases", typeof(DialogueContainer));
        //Debug.Log("Se han obtenido los siguientes casos:");
        contextDescription.text = "Casos detectados: ";
        string casePath = $"{caso}_{LanguageManager.Instance.languageSelected}";
        foreach (var item in casesArray)
        {
            //Debug.Log($"{item.name}");
            contextDescription.text += $" {item.name}";
            if (item.name == casePath)
            {
                dialogueContainer = item as DialogueContainer;
            }
            //Debug.Log($"{cases[cases.Count-1].name}");
        }

        /*
         #if UNITY_EDITOR

        #endif

        #if UNITY_ANDROID

        #endif
         */
    }

    public void SetNameToCharacters()
    {
        foreach (var item in characteres)
        {
            float randomStartDelay = Random.Range(0, 4.5f);//random init delay
            item.Update(randomStartDelay);

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
        if (texto == null || texto == "" || texto == "Deprecated field (use description node)" || texto == "Audio id")
        {
            Debug.Log($"El texto utilizado no es valido");
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

                //Comprobamos si alguna enfermera ha salido corriendo :')
                SpecialCases.Instance.CheckSituation(situation.SituationName);

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

        StartCoroutine(PlaySimpleDialogue(situation.audioId, nextButtonContext));

        //Debug.Log($"Se ha configurado el contexto de la situacion {situation.SituationName}");

    }

    public void SetUpScreen2() //situation dialogues setUp
    {
        //situation = _situation; //esto tiene que estar configurado ya
        AddTextToRoute($"{situation.SituationName}: {situation.Context}"); //LO GUARDAMOS PARA EL CORREO

        if (LoadDialogues(situation.Guid, situationDialogues)) 
        {
            //RefreshDialogues(situationDialogues, situationDialogueTexts);
            StartCoroutine(PlayDialogues(situationDialogues, situationDialogueTexts, nextButtonSituation));

            //Debug.Log($"Se ha asignado el primer dialogo de la situacion {situation.SituationName}, dialogo = {situationDialogues[0].DialogueText}");
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

        //Debug.Log($"Se ha configurado la screen 3 con la pregunta y las respuestas de la situacion {situation.SituationName}, pregunta {questions[currentQuestion].QuestionName}");
    }

    public void SetUpScreen4(AnswerNodeData answer)
    {
        choosenAnswer = answer;
        if (LoadDialogues(answer.Guid, answerDialogues))
        {

            //RefreshDialogues(answerDialogues, answerDialogueTexts);
            StartCoroutine(PlayDialogues(answerDialogues, answerDialogueTexts, nextButtonAnswer));

            //Debug.Log($"Se ha asignado el primer dialogo de la answer {answer.AnswerName}, dialogo = {answerDialogues[0].DialogueText}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la answer {answer.AnswerName}");
        }
    }

    public void SetUpScreen5(AnswerNodeData answer)
    {

        if (isValid(answer.Feedback))
        {
            StartCoroutine(PlaySimpleDialogue(answer.audioId, nextButtonFeedback));
        }

        StartCoroutine(SpecialCases.Instance.ExitRoom());
        
        feedbackText.text = $"{answer.Feedback} \n Score: {totalScore}";
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
            dialoguesUI[i].text = "";
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
                //Debug.Log($"Vamos a resaltar el dialogo del speaker {dialoguesData[currentDialogue + i].Speaker}");
                
                if (dictionaryCharacteres.ContainsKey(Translate(dialoguesData[currentDialogue + i].Speaker)))
                {
                    dictionaryCharacteres[Translate(dialoguesData[currentDialogue + i].Speaker)].SetBool("animFinished", true);
                    SetColorName(0f, Translate(dialoguesData[currentDialogue + i].Speaker));
                }
                else
                {
                    if (dialoguesData[currentDialogue + i].Speaker != "Narrador" && dialoguesData[currentDialogue + i].Speaker != "Narrator")
                    {
                        Debug.Log($"No se encontro el speaker {Translate(dialoguesData[currentDialogue + i].Speaker)}");
                    }
                }
                //Antes de empezar el siguiente dialogo y antes de comenzar el siguiente comprobamos si hay que hacer alguna accion concreta:
                SpecialCases.Instance.CheckSpecialEvent(dialoguesData[currentDialogue + i].DialogueName);
                while (SpecialCases.Instance.playingAnimation)
                {
                    yield return null;
                }

            }
            else
            {
                end = true;
            }
        }

        currentDialogue += i;
        nextButton.SetActive(true);
    }

    public IEnumerator PlaySimpleDialogue(string audio, GameObject nextButton)
    {
        nextButton.SetActive(false);
        yield return new WaitForSeconds(PlayAudioOnSpeaker(audio, "Narrator", "Calm"));
        nextButton.SetActive(true);
    }

    public string Translate(string toTranslate) //Spanish to English
    {
        string aux = toTranslate;
        //{ Surgeon1, Surgeon2, CirculatingNurse, AnaesthesiaNurse, InstrumentalistNurse, Anaesthesiologist};
        //Scene 7: Endoscopist1, Endoscopist2, EndoscopyNurse, AnaesthesiaNurse, Anaesthesiologist
        switch (toTranslate)
        {
            case "Endoscopista 1":
            case "Endoscopist 1":
                aux = "Endoscopist1";
                break;
            case "Anastesiólogo":
            case "Anesthesiologist":
                aux = "Anaesthesiologist";
                break;
            case "Enfermera de endoscopia":
            case "Endoscopy nurse":
            case "Endoscopy Nurse":
                aux = "EndoscopyNurse";
                break;
            case "Endoscopista 2":
            case "Endoscopist 2":
                aux = "Endoscopist2";
                break;
            case "Secretaria":
            case "Secretario":
            case "Secretary":
                aux = "Secretary";
                break;
            case "Enfermera de anestesia":
                aux = "AnaesthesiaNurse";
                break;
        }


        return aux;
    }

    public void SetColorName(float color, string speaker)
    {
        //Debug.Log($"Vamos a intentar iluminar el nombre de {speaker}");
        speaker = speaker.ToLower();
        switch (speaker)
        {
            case "Endoscopista 1":
            case "Endoscopist 1":
            case "Endoscopist1":
            case "endoscopist1":
            case "endoscopist 1":
                //Debug.Log($"VIluminamos el {speaker}");
                endoescopista1.speed = color;
                break;
            case "Anastesiólogo":
            case "Anaesthesiologist":
            case "Anesthesiologist":
            case "anesthesiologist":
            case "anaesthesiologist":
                //Debug.Log($"VIluminamos el {speaker}");
                anestesiologo.speed = color;
                break;
            case "Enfermera de endoscopia":
            case "Endoscopy nurse":
            case "EndoscopyNurse":
            case "Endoscopy Nurse":
            case "endoscopy nurse":
            case "endoscopynurse":
                //Debug.Log($"VIluminamos el {speaker}");
                enfermeraDeEndoscopia.speed = color;
                break;
            case "Endoscopista 2":
            case "endoscopista 2":
            case "Endoscopist 2":
            case "endoscopist 2":
            case "Endoscopist2":
            case "endoscopist2":
                //Debug.Log($"VIluminamos el {speaker}");
                endoescopista2.speed = color;
                break;
            case "Secretaria":
            case "Secretario":
            case "Secretary":
            case "secretary":

                break;
            case "Enfermera de anestesia":
            case "AnaesthesiaNurse":
            case "anaesthesiaNurse":
            case "anaesthesianurse":
            case "anaesthesia nurse":
                //Debug.Log($"VIluminamos el {speaker}");
                enfermeraDeAnestesia.speed = color;
                break;
        }
    }

    public void SetupTutorial()
    {
      OptionsManager.Instance.ResetScene();
    }

    public void Exit()
    {
        ExitGame.Instance.ExitGameMethod();
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void Testing()
    {
        StartCoroutine(SpecialCases.Instance.ExitRoom());
    }

}
