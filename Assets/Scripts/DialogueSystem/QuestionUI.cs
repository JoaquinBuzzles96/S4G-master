using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    public TextMeshProUGUI description;
    public TextMeshProUGUI timerText;
    public bool answered = false;
    public List<Button> answers = new List<Button>();
    [System.NonSerialized]
    public QuestionNodeData questionData;
    private List<AnswerNodeData> answersData;
    public GameObject AnswerContainer;

    public GameObject countdownBar;

    //Hacer que una imagen siga la barra de tiempo - Juan
    public GameObject imageAmbulance;
    Transform startPosition;
    bool activarMoverImagen;
    bool activarLerpImagen;
    public float speed = 1.0F;
    float startTime;
    float journeyLength;
    public GameObject imageHospital;



    void Start()
    {
        //Juan
        startPosition.transform.position = imageAmbulance.transform.position;
    }


    void Update()
    {

        //Juan
        if (activarMoverImagen)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            imageAmbulance.transform.position = Vector3.Lerp(startPosition.transform.position, imageHospital.transform.position, fractionOfJourney);
        }
    }

    public void SetupQuestion()
    {
        description.text = questionData.Description;
        //Debug.Log($"Setup description: {questionData.Description}");

        UI_Manager.Instance.lastTime = Time.time;
        answersData = UI_Manager.Instance.dialogueContainer.GetQuestionAnswers(questionData.Guid);

        UI_Manager.Instance.AddTextToRoute("To the question: " + questionData.QuestionName + " " + questionData.Description);

        //Debug.Log($"Se ha configurado la pregunta {questionData.QuestionName}, sus respuestas son {answersData[0].AnswerName}");//, {answersData[1].AnswerName}, {answersData[2].AnswerName}, {answersData[3].AnswerName}

        int i = 0;
        //Cargamos todas las respuestas
        for (i = 0; i < answersData.Count; i++)
        {
            //Debug.Log($"Vamos a configurar el slot {i} con {answersData[i].AnswerName}, longitud de answers = {answers.Count}, longitud de answersData = {answersData.Count}");
            answers[i].GetComponent<AnswerUI>().SetupAnswer(answersData[i]);
            //Debug.Log($"El slot {i} tiene la respuesta {answersData[i].AnswerName}");
        }
        
        //Si hubiera menos de 4 el resto los limpiamos
        for (int j = i; j < answers.Count; j++)
        {
            answers[j].GetComponent<AnswerUI>().ClearAnswer();
            //Debug.Log($"El slot {j} esta clear");
        }

        StartCoroutine(UI_Manager.Instance.PlaySimpleDialogue(questionData.audioId, AnswerContainer));

        //Iniciamos el timer:
        StartCoroutine(QuestionTimer());

    }

    IEnumerator QuestionTimer()
    {
        int timer = 25; //segundos que tienes para responder
        countdownBar.transform.localScale = new Vector3(1, 1, 1);
        timerText.text = $"{timer}";


        //esperamos a que acabe el audio antes de empezar el timer:
        yield return new WaitForSeconds(UI_Manager.Instance.audioSource.clip.length + 2.5f);

        //en un futuro esto se sacara del question data, de momento lo seteamos a mano
 
        float normalizedTime = 1f;
        float integerTimer = timer;
        while (normalizedTime > 0f && !answered)
        {
            //Juan
            if (activarLerpImagen == false)
            {
                StartLerpImage();
                activarLerpImagen = true;
            }
            activarMoverImagen = true;

            //UPDATE UI
            //Debug.Log($"% de tiempo: {normalizedTime}");
            normalizedTime -= Time.deltaTime / timer;
            countdownBar.transform.localScale = new Vector3(normalizedTime, 1, 1);
            integerTimer -= Time.deltaTime;
            timerText.text = $"{(int)integerTimer}";

            yield return null; //Igual esto hay que hacerlo sin corroutina
        }

        if (!answered)
        {
            //Pierdes puntos si llegas a este punto
            UI_Manager.Instance.totalScore -= 5;
        }

        //Para la siguiente
        answered = false;
    }

    //Juan
    void StartLerpImage()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition.position, imageHospital.transform.position);
    }
    
    public void ResetImage()
    {
        //Deja la imagen en su sitio inicial.
        imageAmbulance.transform.position = startPosition.position;

        activarLerpImagen = false;
        activarMoverImagen = false;
    }

}
