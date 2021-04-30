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
    private List<DialogueNodeData> dialogues = new List<DialogueNodeData>();

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void SetupAnswer(AnswerNodeData _answerData, string CORRECT_GUID = null)
    {
        this.gameObject.SetActive(true);
        answerData = _answerData;

        if (UI_Manager.Instance.LoadDialogues(answerData.Guid, dialogues))
        {
            description.text = $"(id: {dialogues[0].DialogueName}) {dialogues[0].DialogueText}"; 
            //Debug.Log($"Se ha asignado el primer dialogo de la respuesta {answerData.AnswerName}, dialogo = {description.text}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la respuesta {answerData.AnswerName}");
            description.text = answerData.Feedback;
        }

        if (CORRECT_GUID != null)
        {
            answerData.Guid = CORRECT_GUID; //next situation no se utiliza 
            //nextSituation = UI_Manager.Instance.dialogueContainer.GetNextSituation(CORRECT_GUID);
        }
        else
        {
            nextSituation = UI_Manager.Instance.dialogueContainer.GetNextSituation(answerData.Guid);
        }
        
        //Debug.Log($"Añadimos la respuesta {answerData.Description}");
    }

    public void ClearAnswer()
    {
        description.text = "";
        this.gameObject.SetActive(false);
    }

    public void OnSelectAnswer()
    {
        QuestionUI.Instance.arrowBlink.ResetValues();
        UI_Manager.Instance.AddTextToRoute("The answer was: " + answerData.AnswerName + "\n" + "Time to answer: " + Mathf.RoundToInt(Time.time - UI_Manager.Instance.lastTime) + " seconds.\n");
        UI_Manager.Instance.totalScore += answerData.score;
        //UI_Manager.Instance.screen2.GetComponent<QuestionUI>().answered = true;

        UI_Manager.Instance.ToScreen4(answerData, UI_Manager.Instance.screen3);
    }

    public bool AudioIsValid(string _audioId)
    {
        bool isValid = true;

        if (_audioId == "" || _audioId == "Audio id" || _audioId == null)
        {
            isValid = false;
        }

        return isValid;
    }

    public bool SpeakerIsValid(string _speaker)
    {
        bool isValid = false;
        
        if (_speaker == "Surgeon1" || _speaker == "Surgeon2" || _speaker == "CirculatingNurse" || _speaker == "AnaesthesiaNurse" || _speaker == "InstrumentalistNurse" || _speaker == "Anaesthesiologist")
        {
            isValid = true;
        }

        return isValid;
    }
}
