﻿/*  ------ S4Game Consortium --------------
*  This library is free software; you can redistribute it and/or
*  modify it under the terms of the GNU Lesser General Public
*  License as published by the Free Software Foundation; either
*  version 3 of the License, or (at your option) any later version.
*  This library is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
*  Lesser General Public License for more details.
*  You should have received a copy of the GNU Lesser General Public
*  License along with this library; if not, write to the Free Software
*  CCMIJU, Carretera Nacional 521, Km 41.8 – 1007, Cáceres, Spain*/
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
        this.gameObject.transform.parent.gameObject.SetActive(true);
        answerData = _answerData;

        if (UI_Manager.Instance.LoadDialogues(answerData.Guid, dialogues))
        {
            //description.text = $"(id: {questionData.QuestionName}) {LanguageManager.Instance.GetQuestionSpeaker(questionData)}: {LanguageManager.Instance.GetQuestionDescription(questionData)}";
            //Definitivo --> description.text = $"{LanguageManager.Instance.GetDialogueSpeaker(dialogues[0])}: {LanguageManager.Instance.GetDialogueText(dialogues[0])}"; 
            //description.text = $"(id: {dialogues[0].DialogueName}) {LanguageManager.Instance.GetDialogueSpeaker(dialogues[0])}: {LanguageManager.Instance.GetDialogueText(dialogues[0])}"; 
            description.text = $"{LanguageManager.Instance.GetDialogueSpeaker(dialogues[0])}: {LanguageManager.Instance.GetDialogueText(dialogues[0])}"; 
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
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void OnSelectAnswer()
    {
        UI_Manager.Instance.totalDecisions++;
        if (answerData.score > 0)
        {
            //Comprobar lo que ha tardado, si ha tardado mas de la mitad solo se suma 0.5
            if (Mathf.RoundToInt(Time.time - UI_Manager.Instance.lastTime) > QuestionUI.Instance.timerLimit)
            {
                UI_Manager.Instance.totalCorrectAnswers = UI_Manager.Instance.totalCorrectAnswers + 0.5f;
            }
            else
            {
                UI_Manager.Instance.totalCorrectAnswers = UI_Manager.Instance.totalCorrectAnswers + 1f;
            }


        }
        QuestionUI.Instance.arrowBlink.ResetValues();
        UI_Manager.Instance.AddTextToRoute("The answer was: " + answerData.AnswerName + "\n" + "Time to answer: " + Mathf.RoundToInt(Time.time - UI_Manager.Instance.lastTime) + " seconds.\n Score: " + answerData.score+ "\n");
        UI_Manager.Instance.totalScore += answerData.score;

        //Desactivamos la animacion del que hace la pregunta
        UI_Manager.Instance.DisableCurrentSpeaker();

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
