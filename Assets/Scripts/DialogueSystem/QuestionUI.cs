using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    private static QuestionUI instance = null;

    // Game Instance Singleton
    public static QuestionUI Instance
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
    }

    public TextMeshProUGUI description;
    public TextMeshProUGUI timerText;
    public bool answered = false;
    public List<Button> answers = new List<Button>();
    [System.NonSerialized]
    public QuestionNodeData questionData;
    private List<AnswerNodeData> answersData;
    public GameObject AnswerContainer;

    public GameObject countdownBar;

    //Hacer que una imagen siga la barra de tiempo
    public GameObject imageAmbulance;
    public GameObject imageHospital;
    public Vector3 originalPosition;

    public BlinkObject arrowBlink;

    AnswerNodeData correctAnswer;
    int maxScore;

    int timerLimit = 25;
    int scorePenalty = 5;
    int scorePenalty2 = 5;

    void Start()
    {
        
        //originalPosition = imageAmbulance.transform.position;
        //Debug.Log($"Guardamos la posicion inicial de la ambulancia {originalPosition}, deberia ser 24,22,09, original position position = { imageAmbulance.transform.position}");
    }


    void Update()
    {


    }

    public void SetupQuestion()
    {

        string speaker = questionData.speaker;
        if (speaker == " " || speaker == "Narrador" || speaker == "" || speaker == "Narrator" || string.IsNullOrEmpty(speaker))
        {
            description.text = $"(id: {questionData.QuestionName}) {questionData.Description}";
        }
        else
        {
            description.text = $"(id: {questionData.QuestionName}) {questionData.speaker}: {questionData.Description}";
        }

        //Debug.Log($"Setup description: {questionData.Description}");

        UI_Manager.Instance.lastTime = Time.time;
        answersData = UI_Manager.Instance.dialogueContainer.GetQuestionAnswers(questionData.Guid);

        UI_Manager.Instance.AddTextToRoute("To the question: " + questionData.QuestionName + " " + questionData.Description);

        //Debug.Log($"Se ha configurado la pregunta {questionData.QuestionName}, sus respuestas son {answersData[0].AnswerName}");//, {answersData[1].AnswerName}, {answersData[2].AnswerName}, {answersData[3].AnswerName}

        int i = 0;
        correctAnswer = null;
        maxScore = 0;
        //Cargamos todas las respuestas
        for (i = 0; i < answersData.Count; i++)
        {
            //Debug.Log($"Vamos a configurar el slot {i} con {answersData[i].AnswerName}, longitud de answers = {answers.Count}, longitud de answersData = {answersData.Count}");
            answers[i].GetComponent<AnswerUI>().SetupAnswer(answersData[i]);

            if (answersData[i].score > maxScore) //si es la correcta la guardamos por si no contesta pasar automaticamente cuando se acabe el tiempo (nos quedamos con la mas correcta)
            {
                correctAnswer = answersData[i];
                maxScore = answersData[i].score;
            }
            //Debug.Log($"El slot {i} tiene la respuesta {answersData[i].AnswerName}");
        }

        if (correctAnswer == null)
        {
            correctAnswer = answersData[0]; //por defecto
        }
        
        //Si hubiera menos de 4 el resto los limpiamos
        for (int j = i; j < answers.Count; j++)
        {
            answers[j].GetComponent<AnswerUI>().ClearAnswer();
            //Debug.Log($"El slot {j} esta clear");
        }

        if (originalPosition != Vector3.zero)
        {
            imageAmbulance.transform.position = originalPosition;
        }
        else
        {
            originalPosition = imageAmbulance.transform.position;
        }

        StartCoroutine(UI_Manager.Instance.PlaySimpleDialogue(questionData.audioId, AnswerContainer));

        //Iniciamos el timer:
        StartCoroutine(QuestionTimer());

    }

    IEnumerator QuestionTimer()
    {
        int timer = timerLimit * 2; //segundos que tienes para responder (25 * 2)
        countdownBar.transform.localScale = new Vector3(1, 1, 1);
        timerText.text = $"{timer}";

        bool penalty1 = true;

        //esperamos a que acabe el audio antes de empezar el timer:
        yield return new WaitForSeconds(UI_Manager.Instance.audioSource.clip.length + 1f);

        //en un futuro esto se sacara del question data, de momento lo seteamos a mano
 
        float normalizedTime = 1f;
        float integerTimer = timer;

        float t = 0; //representara la distancia recorrida
        Vector3 startPosition_ = imageAmbulance.transform.position;// originalPosition;
        Vector3 target = imageHospital.transform.position;
        float timeToReachTarget = timer;

        while (normalizedTime > 0f && !answered)
        {

            //Ambulancia
            t += Time.deltaTime / timeToReachTarget;
            imageAmbulance.transform.position = Vector3.Lerp(startPosition_, target, t);

            //UPDATE UI
            //Debug.Log($"% de tiempo: {normalizedTime}");
            normalizedTime -= Time.deltaTime / timer;
            countdownBar.transform.localScale = new Vector3(normalizedTime, 1, 1);
            integerTimer -= Time.deltaTime;
            timerText.text = $"{(int)integerTimer}";

            if (integerTimer <= timerLimit) //Cuando va por la mitad 
            {
                //restar la primera penalizacion
                if (penalty1)
                {
                    UI_Manager.Instance.totalScore -= scorePenalty;
                    penalty1 = false; //solo la restamos una vez

                    //Parpadeo
                    arrowBlink.isActive = true;
                }

                if (integerTimer <= (timerLimit/2) && arrowBlink.speed == arrowBlink.originalSpeed)
                {
                    //aumentamos la velocidad del parpadeo
                    arrowBlink.speed = arrowBlink.speed * 2;
                }
            }


            yield return null;
        }

        arrowBlink.ResetValues();
        //Para la siguiente
        answered = false;
        penalty1 = true;


        if (!answered)
        {
            //Pierdes puntos si llegas a este punto
            UI_Manager.Instance.totalScore -= scorePenalty2;
            AutomaticAnswer();
        }
    }

    void AutomaticAnswer() //No suma puntos
    {
        UI_Manager.Instance.AddTextToRoute($" Didn't aswered. The automatic answer was: {correctAnswer.AnswerName} \n" + "Time to answer: " + Mathf.RoundToInt(Time.time - UI_Manager.Instance.lastTime) + " seconds.\n");

        UI_Manager.Instance.ToScreen4(correctAnswer, UI_Manager.Instance.screen3);
    }

}
