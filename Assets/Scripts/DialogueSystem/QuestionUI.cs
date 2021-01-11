using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    public TextMeshProUGUI description;
    public List<Button> answers = new List<Button>();
    [System.NonSerialized]
    public QuestionNodeData questionData;
    private List<AnswerNodeData> answersData;

    void Start()
    {

    }


    void Update()
    {

    }

    public void SetupQuestion()
    {
        description.text = questionData.Description;
        answersData = UI_Manager.Instance.dialogueContainer.GetQuestionAnswers(questionData.Guid);

        UI_Manager.Instance.AddTextToRoute("To the question: " + questionData.QuestionName + " " + questionData.Description);

        Debug.Log($"Se ha configurado la pregunta {questionData.QuestionName}, sus respuestas son {answersData[0].AnswerName}, {answersData[1].AnswerName}, {answersData[2].AnswerName}, {answersData[3].AnswerName}");

        int i = 0;
        for (i = 0; i < answersData.Count; i++)
        {
            //Debug.Log($"Vamos a configurar el slot {i} con {answersData[i].AnswerName}, longitud de answers = {answers.Count}, longitud de answersData = {answersData.Count}");
            answers[i].GetComponent<AnswerUI>().SetupAnswer(answersData[i]);
        }
        
        for (int j = i; j < answers.Count; j++)
        {
            answers[i].GetComponent<AnswerUI>().ClearAnswer();
        }
        
    }
}
