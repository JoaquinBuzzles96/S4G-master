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
