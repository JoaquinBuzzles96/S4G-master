using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public enum Status {Situation, Answer };

public enum Cases { DefaultCase, Case3, Case9, Case5, Case6, Case7 };

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
    private Status currentStatus = Status.Situation;
    public Cases currentCase = Cases.DefaultCase;
    public DialogueContainer dialogueContainer;

    //Arrows
    public GameObject generalArrow;

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

    public GameObject ExitButton;

    public List<Animator> characteres;

    public GameObject PANEL_IMAGE;

    public Dictionary<string, Animator> dictionaryCharacteres = new Dictionary<string, Animator>();

    public string caso;

    int lastQuestion;
    int currentQuestion;
    string currentSpeaker;

    List<string> playedSituationsList;

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
    public string playereRoute = "";
    public int totalScore; 
    public int totalCorrectAnswers; 
    public int totalDecisions; 
    public float lastTime;


    //Camera positions
    public GameObject cameraPositionFirstPerson;
    public GameObject cameraPositionThirdPerson;
    public GameObject cameraCurrentPos;

    bool isStopped = false;
    bool isExitActive = false;

    SituationNodeData lastSituation;

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
        //Testing
        //SendMail.Instance.SaveCSV("example testing");
        //End testing
        playedSituationsList = new List<string>();
        currentSpeaker = "";
        GetDialogueContainerLanguage();
        SetUpCharacrteres();
        lastQuestion = -1;
        totalScore = 0;
        totalCorrectAnswers = 0;
        totalDecisions = 0;
        lastSituation = null;
        SetUpContext(dialogueContainer.GetFirstSituation());
    }

    void Update()
    {
        CheckRandomAnims();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            /*
            if (isExitActive)
            {
                ExitButton.SetActive(false);
            }
            else
            {
                ExitButton.SetActive(true);
            }
            
            isExitActive = !isExitActive;
            */
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StopGame();
        }
    }

    public void StopGame()
    {
        if (isStopped)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }

        isStopped = !isStopped;
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
            questions = dialogueContainer.GetSituationQuestions(situation.Guid);

            AddTextToRoute(situation.SituationName);

            //Debug.Log($"Se ha configurado la situacion {situation.SituationName}, tiene {questions.Count} preguntas posibles");
            //foreach (var item in questions){Debug.Log($"{item.QuestionName}");}
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la situacion {situation.SituationName}");
            //situationDialogueTexts.text = situation.Context;

            //En este caso vamos directos a la pregunta //TODO

        }


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
        //screen1.SetActive(true);
        PANEL_IMAGE.SetActive(false);
    }

    public void ToScreen2(GameObject originScreen) //Dialogue situation
    {
        PANEL_IMAGE.SetActive(false);
        SetUpScreen2(); //Actualizamos los dialogos de la pantalla
        originScreen.SetActive(false);
        //screen2.SetActive(true); //TEMPORAL, TESTING
        
    }

    public void ToScreen3(GameObject originScreen) // Answer choose
    {
        //if (!CheckMoreDialogues(situationDialogues, situationDialogueTexts, nextButtonSituation)) //si no hay mas dialogos entra aqui
        //{
        originScreen.SetActive(false);
        screen3.SetActive(true);
        PANEL_IMAGE.SetActive(true);
        SetUpScreen3();
        
        //}
    }

    public void ToScreen4(AnswerNodeData answer,GameObject originScreen) // Answer Dialogue
    {
        originScreen.SetActive(false);
        //screen4.SetActive(true);//TEMPORAL, TESTING
        SetUpScreen4(answer); 
        PANEL_IMAGE.SetActive(false);
    }

    public void ToScreen5(AnswerNodeData answer, GameObject originScreen) //Feedback
    {
        originScreen.SetActive(false);
        screen5.SetActive(true);
        PANEL_IMAGE.SetActive(true);
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
        screen2.GetComponent<QuestionUI>().SetupQuestion(false);
        LanguageManager.Instance.UpdateLanguage();
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
            SetColorName(0.75f, speaker, dictionaryCharacteres[speaker], Color.green);
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
            return audioSource.clip.length + 1f;
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

    public string GetAudioPath(string _audio) //TODO: Esta ruta no la esta pillando en las builds
    {
        //string path = $"Audio/Case5_EN/{_audio}";
        string path = $"Audio/Case5_EN/Audio1"; //default value
        if (isValid(caso) && LanguageManager.Instance != null && isValid(_audio))
        {
            if (isValid(LanguageManager.Instance.languageSelected))
            {
                //path = $"Audio/{caso}_{LanguageManager.Instance.languageSelected}/{_audio}";
                path = $"Audio/{caso}_EN/{_audio}";
            }
            else
            {
                Debug.Log(" 1 - No se ha encontrado el audio por lo que asignamos uno por defecto--> " + $"Audio/{caso}_EN/{_audio}");
                path = $"Audio/Case5_EN/Audio1";
            }
            //#if PLATFORM_ANDROID
            //#endif

            //#if UNITY_EDITOR_WIN

            //#endif
        }
        else
        {
            Debug.Log("2- No se ha encontrado el audio por lo que asignamos uno por defecto No se ha encontrado --> " + $"Audio/{caso}_EN/{_audio}");
            path = $"Audio/Case5_EN/Audio1";
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
            Debug.Log($"El Languague manager no es nulo, contiene es caso: {LanguageManager.Instance.caseSelected} y languague {LanguageManager.Instance.languageSelected}");
            caso = LanguageManager.Instance.caseSelected;
            //path = Application.streamingAssetsPath + $"/Resources/Cases/{caso}_{LanguageManager.Instance.languageSelected}";
            path = Application.streamingAssetsPath + $"/Resources/Cases/{caso}_EN";

            //Este valor se asigna para mas adelante utilizar unos eventos y animaciones especificas u otras
            switch (caso)
            {
                case "CASE3":
                    currentCase = Cases.Case3;
                    break;
                case "CASE5":
                    currentCase = Cases.Case5;
                    break;
                case "CASE6":
                    currentCase = Cases.Case6;
                    break;
                case "CASE7":
                    currentCase = Cases.Case7;
                    break;
                case "CASE9":
                    currentCase = Cases.Case9;
                    break;
                default:
                    currentCase = Cases.DefaultCase; //sin nada de animaciones especificas
                    break;
            }

            //dialogueContainer = Resources.Load($"Cases/{caso}_{LanguageManager.Instance.languageSelected}") as DialogueContainer;
            //Debug.Log($"Cargamos el case Cases/{caso}_{LanguageManager.Instance.languageSelected}");
        }
        else
        {
            path = Application.streamingAssetsPath + $"/Resources/Cases/testing2";
            //Debug.Log($"No se ha encontrado el languague manager");
            //dialogueContainer = Resources.Load($"Cases/testing2") as DialogueContainer;
        }

        LanguageManager.Instance.UpdateLanguage(); //ACTUALIZAMOS POR SI ACASO

        if (LanguageManager.Instance.isThirdPerson || currentCase == Cases.Case5 || currentCase == Cases.Case6 || currentCase == Cases.Case7)
        {
            //ponemos la camara en tercera persona
            cameraCurrentPos.transform.position = cameraPositionThirdPerson.transform.position;
            cameraCurrentPos.transform.rotation = cameraPositionThirdPerson.transform.rotation;

            //Si estamos en tercera persona activamos el face to player del panel de respuestas:
            //this.gameObject.GetComponent<FrontPlayer>().enabled = true;

        }
        else
        {
            //ponemos la camara en primera persona
            cameraCurrentPos.transform.position = cameraPositionFirstPerson.transform.position;
            cameraCurrentPos.transform.rotation = cameraPositionFirstPerson.transform.rotation;
            //this.gameObject.GetComponent<FrontPlayer>().enabled = false;
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
        contextDescription.text = "Si estas viendo esto es que el caso no ha cargado bien, ve al UI_MANAGER";
        string casePath = $"{caso}_EN";
        //string casePath = $"{caso}_{LanguageManager.Instance.languageSelected}";
        foreach (var item in casesArray)
        {
            //Debug.Log($"{item.name}");
            //contextDescription.text += $" {item.name}";
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
        //Metemos en el diccionario los personajes de todas las situaciones
        foreach (var item in characteres)
        {
            float randomStartDelay = Random.Range(0, 4.5f);//random init delay
            item.Update(randomStartDelay);

            dictionaryCharacteres.Add(item.gameObject.name, item);
            item.gameObject.SetActive(false);
            //Debug.Log($"Se ha añadido el {item.gameObject.name} al diccionario");
        }
    }

    public void SetUpCharacrteres()
    {
        //Metemos en el diccionario los personajes de todas las situaciones
        SetNameToCharacters();

        //Llegados a este punto damos por supuesto que en el diccionario estan todos los personajes

        //En funcion del caso activamos los personajes que participaran
        if (currentCase == Cases.Case3)
        {
            dictionaryCharacteres["Endoscopist1"].gameObject.SetActive(true);
            dictionaryCharacteres["Endoscopist2"].gameObject.SetActive(true);
            dictionaryCharacteres["EndoscopyNurse"].gameObject.SetActive(true);
            dictionaryCharacteres["AnaesthesiaNurse"].gameObject.SetActive(true);
            dictionaryCharacteres["Anaesthesiologist"].gameObject.SetActive(true);
            dictionaryCharacteres["Secretary"].gameObject.SetActive(true);
            dictionaryCharacteres["EndoscopyNurseExtra"].gameObject.SetActive(true);
        }
        else if (currentCase == Cases.Case5)
        {
            dictionaryCharacteres["HeadSurgeon"].gameObject.SetActive(true);
            dictionaryCharacteres["HeadSurgeon"].gameObject.GetComponent<CapsuleCollider>().enabled = true; //activamos el collider para que el raycast funcione
            dictionaryCharacteres["AssistantSurgeon"].gameObject.SetActive(true); // Hara de equivalente a surgeon1
            dictionaryCharacteres["CirculatingNurse"].gameObject.SetActive(true);
            dictionaryCharacteres["AnaesthesiaNurse"].gameObject.SetActive(true);
            dictionaryCharacteres["Anaesthesiologist"].gameObject.SetActive(true);
            dictionaryCharacteres["Secretary"].gameObject.SetActive(true);
            dictionaryCharacteres["InstrumentalistNurse"].gameObject.SetActive(true); // De estos igual hay que poner varios
            Case3Resources.Instance.mask.SetActive(true);
            //dictionaryCharacteres["Patient"].gameObject.SetActive(true);
        }
        else if (currentCase == Cases.Case6)
        {
            Case3Resources.Instance.mask.SetActive(true);
            dictionaryCharacteres["Urologist"].gameObject.SetActive(true);
            dictionaryCharacteres["AssistantSurgeon"].gameObject.SetActive(true);
            dictionaryCharacteres["InstrumentalistNurse"].gameObject.SetActive(true);
            dictionaryCharacteres["Anaesthesiologist"].gameObject.SetActive(true);
            dictionaryCharacteres["HeadSurgeon"].gameObject.SetActive(true);
            dictionaryCharacteres["CirculatingNurse"].gameObject.SetActive(true);
            //dictionaryCharacteres["Patient"].gameObject.SetActive(true);
        }
        else if (currentCase == Cases.Case7)
        {
            //TODO
            Case3Resources.Instance.mask.SetActive(true);
            dictionaryCharacteres["MainSurgeon"].gameObject.SetActive(true); //todo traductor
            dictionaryCharacteres["Anaesthesiologist"].gameObject.SetActive(true);
            dictionaryCharacteres["ResponsibleNurse"].gameObject.SetActive(true); //todo traductor
            dictionaryCharacteres["InstrumentalistNurse"].gameObject.SetActive(true); //todo traductor
            dictionaryCharacteres["CameraAssistant"].gameObject.SetActive(true); //todo traductor
            //dictionaryCharacteres["Patient"].gameObject.SetActive(true);
        }
        else if (currentCase == Cases.Case9)
        {
            dictionaryCharacteres["Endoscopist1"].gameObject.SetActive(true);
            dictionaryCharacteres["Endoscopist2"].gameObject.SetActive(true); // Hara de equivalente a surgeon1
            dictionaryCharacteres["EndoscopyNurse"].gameObject.SetActive(true);
            Case3Resources.Instance.mask.SetActive(true);
            dictionaryCharacteres["Secretary"].gameObject.SetActive(true);
            dictionaryCharacteres["Student"].gameObject.SetActive(true); 
            dictionaryCharacteres["Student_2"].gameObject.SetActive(true); 
            dictionaryCharacteres["Student_3"].gameObject.SetActive(true); 
            dictionaryCharacteres["Student_4"].gameObject.SetActive(true); 
            //dictionaryCharacteres["Student2"].gameObject.SetActive(true); // De estos igual hay que poner varios
            //dictionaryCharacteres["Student3"].gameObject.SetActive(true); // De estos igual hay que poner varios
            //dictionaryCharacteres["Student4"].gameObject.SetActive(true); // De estos igual hay que poner varios
            //dictionaryCharacteres["Patient"].gameObject.SetActive(true);
        }
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
        //if (!CheckMoreDialogues(answerDialogues, answerDialogueTexts, nextButtonAnswer)) //si no hay mas dialogos entra aqui
        //{
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

            //Aqui comprobar si la situation ya se ha puesto antes y si el contexto es valido
            if (isValid(LanguageManager.Instance.GetSituationContext(situation)) && !IsSituationAlreadyPlayed(situation.Guid))
            {
                ToScreen1(situation, originScreen);
            }
            else if(IsSituationAlreadyPlayed(situation.Guid)) //situacion repetida
            {
                Debug.Log("La situacion no es valida o ya se ha hecho, por lo tanto vamos directamente a la pantalla");
                ToScreen3(originScreen);

            }
            else//nueva situacion sin contexto
            {
                ToScreen2(originScreen);
            }
        }
       //}
    }

    public void SetUpContext(SituationNodeData _situation)
    {
        PANEL_IMAGE.SetActive(false);
        Debug.Log($"Vamos a cargar la situacion {_situation}");

        situation = _situation;

        if (LanguageManager.Instance.GetSituationContext(situation) != null)
        {
            contextDescription.text = LanguageManager.Instance.GetSituationContext(situation);
            playedSituationsList.Add(situation.Guid);
        }
        else
        {
            contextDescription.text = "No se ha encontrado el contexto";
        }
        
        nextButtonContext.SetActive(false);
        StartCoroutine(PlaySimpleDialogue(situation.audioId)); //TESTING: , nextButtonContext

        //Debug.Log($"Se ha configurado el contexto de la situacion {situation.SituationName}");

    }

    public void SetUpScreen2() //situation dialogues setUp
    {
        //situation = _situation; //esto tiene que estar configurado ya
        AddTextToRoute($"{situation.SituationName}: {LanguageManager.Instance.GetSituationContext(situation)}"); //LO GUARDAMOS PARA EL CORREO

        if (LoadDialogues(situation.Guid, situationDialogues)) 
        {
            //RefreshDialogues(situationDialogues, situationDialogueTexts);
            currentStatus = Status.Situation;
            StartCoroutine(PlayDialogues(situationDialogues, situationDialogueTexts, nextButtonSituation));

            //Debug.Log($"Se ha asignado el primer dialogo de la situacion {situation.SituationName}, dialogo = {situationDialogues[0].DialogueText}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la situacion {situation.SituationName}");
            ToScreen3(screen2);
        }
    }

    public void SetUpScreen3() //Asignar la pregunta y las 4 posibles respuestas
    {
        generalArrow.GetComponent<LookTarget>().target = this.transform;//PANEL
        generalArrow.SetActive(true); //esta es la flecha que mira al panel
        Debug.Log("INICIAMOS LA CORROUTINA DEL MAIN PANEL");
        StartCoroutine(CheckIfPanelFieldOfView()); //con esta corroutina detectara el collider del panel cuando lo mires y desactivara la flecha 

        bool lastOption = false;

        if (lastSituation != null && lastSituation == situation) //en caso de repetido
        {
            //en este caso questions ya estaba con informacion de la iteracion anterior
            questions.RemoveAt(lastQuestion);//ELIMINAMOS LA ULTIMA QUE HAYA SALIDO

            if (questions.Count == 1) //en caso de quedar solo una tenemos que indicarselo a la pregunta, si fallan se avazara igualmente
            {
                lastOption = true;
            }
        }
        else
        {
            //En este caso si podemos suponer que la situacion que tenemos es la correcta, de modo que no es necesario pasarla por parametro
            questions = dialogueContainer.GetSituationQuestions(situation.Guid); //OBTENEMOS LAS PREGUNTAS DE ESTE NODO
        }

        lastSituation = situation;

        currentQuestion = Random.Range(0, questions.Count); //COGEMOS UNA ALEATORIA

        /*
        if (currentQuestion == lastQuestion) //SI ES LA MISMA QUE LA ANTERIOR CAMBIAMOS --> ESTO HABRA QUE MODIFICARLO PARA QUE SEA UN CJECH SI HSE HA HECHO EN ALGUN MOMENTO
        {
            currentQuestion++;
            currentQuestion = currentQuestion % questions.Count;
            //Debug.Log($"Como la pregunta era la misma que la anterior la cambiamos: current: {currentQuestion} last: {lastQuestion}");
        }
        */
        //Debug.Log("Vamos a configurar el question data");

        screen3.GetComponent<QuestionUI>().questionData = questions[currentQuestion];
        screen3.GetComponent<QuestionUI>().SetupQuestion(lastOption, situation);

        lastQuestion = currentQuestion;


        //Poner aqui que la flecha espere (en caso de que se quiera implementar, seria necesario ampliar el collider en ese caso)

        //Debug.Log($"Se ha configurado la screen 3 con la pregunta y las respuestas de la situacion {situation.SituationName}, pregunta {questions[currentQuestion].QuestionName}");
    }

    public void SetUpScreen4(AnswerNodeData answer)
    {
        generalArrow.SetActive(false);

        choosenAnswer = answer;
        if (LoadDialogues(answer.Guid, answerDialogues))
        {

            //RefreshDialogues(answerDialogues, answerDialogueTexts);
            currentStatus = Status.Answer;
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
        if (totalScore < 0)
        {
            totalScore = 0;
        }
        if (answer == null) //default feedback
        {
            string feedback = "Your reaction time was too long. During medical procedures, the soft skills should be at appropriate level not to interfere significantly with the flow of the main medical procedure (hard skills). The game is over. Please try it again.";
            feedbackText.text = $"{feedback} \n Score: {totalScore}";
            //Add feedback to email
            //AddTextToRoute("\n Total time playing: " + Mathf.RoundToInt(Time.time / 60) + " minuts and " + Mathf.RoundToInt(Time.time % 60) + " seconds.");
            AddTextToRoute($"Feedback: {feedback}");
            //AddTextToRoute($"Final score: {totalScore}");
        }
        else
        {
            if (isValid(answer.Feedback))
            {
                StartCoroutine(PlaySimpleDialogue(answer.audioId, nextButtonFeedback));
            }
            feedbackText.text = $"{answer.Feedback} \n Score: {totalScore}";
            //Add feedback to email
            AddTextToRoute("\n Total time playing: " + Mathf.RoundToInt(Time.time / 60) + " minuts and " + Mathf.RoundToInt(Time.time % 60) + " seconds.");
            AddTextToRoute($"Feedback: {answer.Feedback}");
            AddTextToRoute($"Final score: {totalScore}");

        }

        //StartCoroutine(SpecialCases.Instance.ExitRoom());
        

        //Send email
        Debug.Log("Enviamos un mail con la infomacion");
        SendMail.Instance.SendEmail();

        ExitButton.SetActive(true);
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

    private bool CheckMoreDialoguesNoNext(List<DialogueNodeData> dialoguesData, List<TextMeshProUGUI> dialoguesUI)
    {
        bool refresh = false;
        if (currentDialogue < dialoguesData.Count) //esto significaria que quedan dialogos sin mostrar
        {
            Clear(dialoguesUI);
            //en este caso hay que hacer un refresh de la UI
            refresh = true;
            //StartCoroutine(PlayDialogues(dialoguesData, dialoguesUI, nextButton));
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
        bool endDialogues = false;

        while (!endDialogues)
        {
            bool end = false;
            int i = 0;
            int j = 0; //como norma general sera igual que la i, pero si hay algun dialogo de narrador esta se incrementara y la i no
                       //Debug.Log($"Entramos y current: {currentDialogue}, i: {i}");
            for (i = 0; i < dialoguesUI.Count && !end; i++)
            {
                if ((currentDialogue + j) < dialoguesData.Count)
                {
                    string tagToCheck = "";

                    //Girarse para escuchar
                    if (dictionaryCharacteres.ContainsKey(Translate(dialoguesData[currentDialogue + j].Speaker)))
                    {
                        tagToCheck = Translate(dialoguesData[currentDialogue + j].Speaker);
                    }
                    else
                    {
                        tagToCheck = "NOT_CHECK";
                    }

                    if (tagToCheck != "NOT_CHECK" && tagToCheck != "Endoscopist1" && tagToCheck != "MainSurgeon")
                    {
                        //Enable arrow
                        generalArrow.SetActive(true);
                        generalArrow.GetComponent<LookTarget>().target = dictionaryCharacteres[Translate(dialoguesData[currentDialogue + j].Speaker)].gameObject.transform;

                        //Debug.Log($"Vamos a buscar al personaje {tagToCheck}");
                        //Activamos el parpadeo hasta que encuentres al personaje
                        SetColorName(0.75f, tagToCheck, dictionaryCharacteres[tagToCheck], Color.red);
                        while (!CheckIfTargetFieldOfView(tagToCheck))
                        {
                            yield return null;
                        }

                        //Disable Arrow
                        generalArrow.SetActive(false);
                    }

                    //Si es el narrador no se pone ni en el panel, ni el audio, ni la animacion de hablar
                    if (Translate(dialoguesData[currentDialogue + j].Speaker) != "Narrator")
                    {
                        //Actualizar Panel
                        dialoguesUI[i].text = $"(id: {dialoguesData[currentDialogue + j].DialogueName}) {LanguageManager.Instance.GetDialogueSpeaker(dialoguesData[currentDialogue + j])}: {LanguageManager.Instance.GetDialogueText(dialoguesData[currentDialogue + j])}";
                        //dialoguesUI[i].text = $"(id: {dialoguesData[currentDialogue + j].DialogueName}) {dialoguesData[currentDialogue + j].Speaker} ({dialoguesData[currentDialogue + j].Mood}): {dialoguesData[currentDialogue + j].DialogueText}";
                        //Email
                        AddTextToRoute($"{LanguageManager.Instance.GetDialogueSpeaker(dialoguesData[currentDialogue + j])}: {LanguageManager.Instance.GetDialogueText(dialoguesData[currentDialogue + j])}");
                        //AddTextToRoute($"{dialoguesData[currentDialogue + j].Speaker} ({dialoguesData[currentDialogue + j].Mood}): {dialoguesData[currentDialogue + j].DialogueText}");
                        //Audio
                        //Aqui o es necesario pasar por el language manager, ya que es a nivel interno
                        yield return new WaitForSeconds(PlayAudioOnSpeaker(dialoguesData[currentDialogue + j].audioId, dialoguesData[currentDialogue + j].Speaker, dialoguesData[currentDialogue + j].Mood));

                        //Quitamos la Animacion al hablar
                        if (dictionaryCharacteres.ContainsKey(Translate(dialoguesData[currentDialogue + j].Speaker)))
                        {
                            dictionaryCharacteres[Translate(dialoguesData[currentDialogue + j].Speaker)].SetBool("animFinished", true);
                            SetColorName(0f, Translate(dialoguesData[currentDialogue + j].Speaker), dictionaryCharacteres[Translate(dialoguesData[currentDialogue + j].Speaker)], Color.green);
                        }
                        else
                        {
                            if (dialoguesData[currentDialogue + j].Speaker != "Narrador" && dialoguesData[currentDialogue + j].Speaker != "Narrator")
                            {
                                Debug.Log($"No se encontro el speaker {Translate(dialoguesData[currentDialogue + j].Speaker)}");
                            }
                        }
                    }
                    else
                    {
                        i--; //lo decrementamos para que no ocupe este slot en la UI y pase al siguiente
                    }
                    //Eventos especiales
                    SpecialCases.Instance.CheckSpecialEvent(dialoguesData[currentDialogue + j].DialogueName);
                    while (SpecialCases.Instance.playingAnimation)
                    {
                        yield return null;
                    }

                    j++;
                }
                else
                {
                    end = true;
                }
            }

            currentDialogue += j;

            if (!CheckMoreDialoguesNoNext(dialoguesData, dialoguesUI))
            {
                endDialogues = true;//Tenemos que ser conscientes de si estamos en una situacion o en una respuesta
            }
            else
            {
                //Debug.Log($"Hay mas dialogos, por lo que limpiamos la pantalla y seguimos. currentDiag = {currentDialogue}");
            }
        }

        //Debug.Log($"No quedan más dialogos, y stamos en status = {currentStatus}");
        //EN FUNCION DE DONDE ESTEMOS VAMOS A LLAMAMOS A CheckIfIsEnd o GoToScreen3
        if (currentStatus == Status.Situation)
        {
            ToScreen3(screen2);
        }
        else if (currentStatus == Status.Answer)
        {
            CheckIfISEnd(screen4);
        }

        //nextButton.SetActive(true);
    }

    bool CheckIfTargetFieldOfView(string tagToCheck)
    {
        bool isTragetInFieldOfView = false;
        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo))
        {
            //Debug.Log($"Chocamos con algo de nombre {hitInfo.collider.gameObject.name} y etiqueta {hitInfo.collider.tag}");
            if (hitInfo.collider.gameObject.name == tagToCheck) //comprobar si esta mirando al doctor que corresponda
            {
                //Debug.Log($"Estamos mirando {tagToCheck}");
                isTragetInFieldOfView = true;
                //Desactivamos el color del nombre en caso de estar resaltado
                SetColorName(0f, tagToCheck, dictionaryCharacteres[tagToCheck], Color.red);
            }
        }

        return isTragetInFieldOfView;
    }

    IEnumerator CheckIfPanelFieldOfView()
    {
        string tagToCheck = "MainPanelNew"; //debemos gestionar que se actyive y desactive el collider //this.gameobject.name
        bool isTragetInFieldOfView = false;
        this.gameObject.GetComponent<BoxCollider>().enabled = true; //nosotros somos el panel, activamos el box collider en caso de estar desactivado
        RaycastHit hitInfo;

        while (!isTragetInFieldOfView)
        {
            //Debug.Log("Estamos buscando el main panel");
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo))
            {
                //Debug.Log($"El raycast ha chocado con {hitInfo.collider.gameObject.name}");
                //Debug.Log($"Chocamos con algo de nombre {hitInfo.collider.gameObject.name} y etiqueta {hitInfo.collider.tag}");
                if (hitInfo.collider.gameObject.name == tagToCheck) //comprobar si esta mirando al doctor que corresponda
                {
                    //Debug.Log($"Estamos mirando {tagToCheck}");
                    isTragetInFieldOfView = true;
                    //Debug.Log("Hemos colisionado con el main panel, desactivamos la flecha");
                    generalArrow.SetActive(false);
                    //desactivamos el collider para que el raycast de oculus funcione
                    hitInfo.collider.gameObject.GetComponent<BoxCollider>().enabled = false; // esto seria lo mismo: this.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
            }

            yield return null;
        }

    }

    public IEnumerator PlaySimpleDialogue(string audio, GameObject nextButton = null, string speaker = null)
    {
        if (speaker == null)
        {
            speaker = "Narrator";
        }
        else
        {
            speaker = Translate(speaker);
        }

        currentSpeaker = speaker;

        if (nextButton != null)
        {
            yield return new WaitForSeconds(PlayAudioOnSpeaker(audio, speaker, "Calm"));

            if (speaker != "Narrator")
            {
                if (dictionaryCharacteres.ContainsKey(Translate(speaker)))
                {
                    dictionaryCharacteres[Translate(speaker)].SetBool("animFinished", true);
                    SetColorName(0f, Translate(speaker), dictionaryCharacteres[speaker], Color.green);
                }
            }

            nextButton.SetActive(true);
        }
        else //TESTING: En caso de no querer eliminar el boton de start eliminamos esto
        {
            yield return new WaitForSeconds(PlayAudioOnSpeaker(audio, speaker, "Calm"));
            //Si estamos en el context, que es cuando no hay next button, entonces vamos automaticamente a la situacion
            if (speaker != "Narrator")
            {
                if (dictionaryCharacteres.ContainsKey(Translate(speaker)))
                {
                    dictionaryCharacteres[Translate(speaker)].SetBool("animFinished", true);
                    SetColorName(0f, Translate(speaker), dictionaryCharacteres[speaker], Color.green);
                }
            }

            ToScreen2(screen1);
        }
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
            case "Anaesthesia Nurse":
            case "Anaesthesia nurse":
                aux = "AnaesthesiaNurse";
                break;
            case "Circulating Nurse":
            case "Circulating nurse":
            case "Circulatingnurse":
                aux = "CirculatingNurse";
                break;
            case "Head Surgeon":
            case "Head surgeon":
            case "Headsurgeon":
                aux = "HeadSurgeon";
                break;
            case "Assistant Surgeon":
            case "Assistant surgeon":
                aux = "AssistantSurgeon";
                break;
            case "Instrumentalist Nurse":
            case "Instrumentalist nurse":
            case "Instrumentalist":
            case "Instrumentist":
            case "InstrumentistSurgeon":
            case "Instrumentist Surgeon":
                //aux = "InstrumentistSurgeon";
                aux = "InstrumentalistNurse";
                break;
            case "Camera Assistant":
            case "Camera assistant":
            case "camera assistant":
            case "camera Assistant":
                aux = "CameraAssistant";
                break;
            case "Responsible Nurse":
            case "Responsible nurse":
            case "responsible nurse":
                aux = "ResponsibleNurse";
                break;
            case "Main Surgeon":
            case "Main surgeon":
            case "Surgeon":
                aux = "MainSurgeon";
                break;
            case "Urologist":
            case "urologist":
                aux = "Urologist";
                break;
            case "Narrador":
                aux = "Narrator";
                break;
        }

        return aux;
    }

    public void SetColorName(float speed, string speaker, Animator animator, Color colorBlink)
    {
        //Debug.Log($"Vamos a intentar iluminar el nombre de {speaker}");
        speaker = speaker.ToLower();

        if (speaker == "Secretary" || speaker == "secretary")
        {
            //nada
        }
        else
        {
            animator.gameObject.GetComponent<TalkAnim>().speed = speed;
            animator.gameObject.GetComponent<TalkAnim>().colorBlink = colorBlink;
        }
       
        /*
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
            case "CirculatingNurse":
                break;
            case "EndoscopyNurseExtra":
                break;
            case "InstrumentalistNurse":
                break;
            case "AssistantSurgeon":
                break;
            case "HeadSurgeon":
                break;
            case "ResponsibleNurse":
                break;
            case "CameraAssistant":
                break;

        }*/
    }

    public void SetupTutorial()
    {
      OptionsManager.Instance.ResetScene();
    }

    public void Exit()
    {
        playedSituationsList.Clear();
        ExitGame.Instance.ExitGameMethod();
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void Testing()
    {
        SendMail.Instance.SendEmail();
    }

    public bool IsSituationAlreadyPlayed(string situation)
    {
        bool found = false;

        for (int i = 0; i < playedSituationsList.Count && !found; i++)
        {
            if (playedSituationsList[i] == situation)
            {
                found = true;
            }
        }

        return found;
    }

    public void DisableCurrentSpeaker()
    {
        if (currentSpeaker != null && currentSpeaker != "Narrator")
        {
            if (dictionaryCharacteres.ContainsKey(Translate(currentSpeaker)))
            {
                dictionaryCharacteres[Translate(currentSpeaker)].SetBool("animFinished", true);
                SetColorName(0f, Translate(currentSpeaker), dictionaryCharacteres[currentSpeaker], Color.green);
            }
        }
    }

    public string CorrectSpeaker(string speaker)
    {

        switch (speaker)
        {
            case "Head Surgeon":
                return "Chief Surgeon";
            case "Main Surgeon":
                return "Chief Surgeon";
            case "Camera Assistant":
                return "Assistant Surgeon";
            case "Nurse Anaesthetist":
                return "Anaesthetic Nurse";
            case "Student":
                return "Medical Student";
        }

        //Si no encuentra ninguno de los anteriores se devuelve el mismo nombre
        return speaker;
    }

}
