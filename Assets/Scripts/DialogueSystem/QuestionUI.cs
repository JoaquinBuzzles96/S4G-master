using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    public TextMeshProUGUI description;
    public List<Button> answers;
    [System.NonSerialized]
    public Question questionData;

    void Start()
    {
        answers = new List<Button>();
    }


    void Update()
    {
        //Answer aux = questionData.posibleAnswers[0];
    }

    public void SetupQuestion()
    {
        description.text = questionData.Description;
        int i;
        for (i = 0; i < questionData.posibleAnswers.Count; i++)
        {
            answers[i].GetComponent<AnswerUI>().SetupAnswer(questionData.posibleAnswers[i]);
        }

        for (int j = i; j < answers.Count; j++)
        {
            answers[i].GetComponent<AnswerUI>().ClearAnswer();
        }
    }
}
