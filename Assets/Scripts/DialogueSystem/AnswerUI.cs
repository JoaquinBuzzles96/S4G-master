using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerUI : MonoBehaviour
{
    [System.NonSerialized]
    public AnswerNodeData answerData;
    public TextMeshProUGUI description;
    public SituationNodeData nextSituation;

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void SetupAnswer(AnswerNodeData _answerData)
    {
        this.gameObject.SetActive(true);
        answerData = _answerData;
        description.text = answerData.Description;
        nextSituation = UI_Manager.Instance.dialogueContainer.GetNextSituation(answerData.Guid);
    }

    public void ClearAnswer()
    {
        description.text = "";
        this.gameObject.SetActive(false);
    }

    public void OnSelectAnswer()
    {
        if (answerData.IsEnd)
        {
            Debug.Log("Enhorabuena, has acabado la simulacion :D");
        }
        else
        {
            if (nextSituation == null)
            {
                Debug.Log("No se ha asignado la siguiente situacion");
            }
            else
            {
                if (answerData.IsCorrect)
                {
                    Debug.Log($"La respuesta {answerData.AnswerName} es correcta, vamos a la situacion {nextSituation.SituationName}");
                }
                else
                {
                    Debug.Log($"La respuesta {answerData.AnswerName} es erronea, vamos a la situacion {nextSituation.SituationName}");
                }

                UI_Manager.Instance.ToScreen1(nextSituation);
            }
        }

        //Debug.Log("Has seleccionado una respuesta");

        /*
        if (answerData.isCorrect)
        {
            Debug.Log("Es correcto :D");
        }
        else
        {
            Debug.Log("Fallaste wey!");
        }
        //Debug.Log($"AnswerData --> {answerData.name}");
        */
    }
}
