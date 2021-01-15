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

    public void SetupAnswer(AnswerNodeData _answerData)
    {
        this.gameObject.SetActive(true);
        answerData = _answerData;

        if (UI_Manager.Instance.LoadDialogues(answerData.Guid, dialogues))
        {
            description.text = dialogues[0].DialogueText;
            Debug.Log($"Se ha asignado el primer dialogo de la respuesta {answerData.AnswerName}, dialogo = {description.text}");
        }
        else
        {
            Debug.LogError($"No se han encontrado dialogos para la respuesta {answerData.AnswerName}");
            description.text = answerData.Feedback;
        }

        nextSituation = UI_Manager.Instance.dialogueContainer.GetNextSituation(answerData.Guid);
        //Debug.Log($"Añadimos la respuesta {answerData.Description}");
    }

    public void ClearAnswer()
    {
        description.text = "";
        this.gameObject.SetActive(false);
    }

    public void OnSelectAnswer()
    {
        //OLD
        /*
        if (answerData.IsEnd)
        {
            UI_Manager.Instance.AddTextToRoute("\n Total time playing: " + Mathf.RoundToInt(Time.time / 60) + " minuts and " + Mathf.RoundToInt(Time.time % 60) + " seconds.");
            Debug.Log("Enhorabuena, has acabado la simulacion :D");
            // send mail
        }
        else
        {
            UI_Manager.Instance.AddTextToRoute("The answer was: " + answerData.AnswerName + " " + answerData.Feedback + "\n" + "Time to answer: " + Mathf.RoundToInt(Time.time - UI_Manager.Instance.lastTime) + " seconds.\n"); if (nextSituation == null)
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

                //Tanto el audio como la animacion mejor que lo lleve el UI_Manager aunque lo invoquemos desde aqui
                //Reproducir audio correspondiente
                //Reproducir animacion correspondiente
                if (AudioIsValid(answerData.audioId) && SpeakerIsValid(answerData.speaker))
                {
                    UI_Manager.Instance.PlayAudioOnSpeaker(answerData.audioId, answerData.speaker);
                }

                UI_Manager.Instance.ToScreen1(nextSituation);
            }
        }
        */
        if (answerData.IsEnd)
        {
            UI_Manager.Instance.AddTextToRoute("\n Total time playing: " + Mathf.RoundToInt(Time.time / 60) + " minuts and " + Mathf.RoundToInt(Time.time % 60) + " seconds.");
        }
        else 
        {
            UI_Manager.Instance.AddTextToRoute("The answer was: " + answerData.AnswerName + "\n" + "Time to answer: " + Mathf.RoundToInt(Time.time - UI_Manager.Instance.lastTime) + " seconds.\n");
        }

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
