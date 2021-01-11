using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogueContainer : ScriptableObject
{

    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<SituationNodeData> SituationNodeData= new List<SituationNodeData>();
    public List<QuestionNodeData> QuestionNodeData= new List<QuestionNodeData>();
    public List<AnswerNodeData> AnswerNodeData= new List<AnswerNodeData>();
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();



    public SituationNodeData GetFirstSituation()
    {
        foreach (var situation in SituationNodeData)
        {
            if (situation.SituationName == "S1")
            {
                return situation;
            }
        }
        return null;
    }

    public List<QuestionNodeData> GetSituationQuestions(string situationGUID)
    {
        List<QuestionNodeData> listAux = new List<QuestionNodeData>();

        foreach (var situation in NodeLinks)
        {
            if (situationGUID == situation.BaseNodeGuid) //esto significa que tiene o bien un dialogue o bien una question
            {
                var aux = GetQuestion(situation.TargetNodeGuid);
                if (aux != null) //con esto descartamos el posible valor nulo en caso de que no fuese una question
                {
                    listAux.Add(aux);
                } 
            }
        }

        return listAux;
    }

    public QuestionNodeData GetQuestion(string questionGUID)
    {
        foreach (var question in QuestionNodeData)
        {
            if (question.Guid == questionGUID)
            {
                return question;
            }
        }

        return null;
    }

    public List<AnswerNodeData> GetQuestionAnswers(string questionGUID)
    {
        List<AnswerNodeData> listAux = new List<AnswerNodeData>();

        foreach (var question in NodeLinks)
        {
            if (questionGUID == question.BaseNodeGuid)
            {
                listAux.Add(GetAnswer(question.TargetNodeGuid));
            }
        }

        return listAux;
    }

    public AnswerNodeData GetAnswer(string answerGUID)
    {
        foreach (var answer in AnswerNodeData)
        {
            if (answer.Guid == answerGUID)
            {
                return answer;
            }
        }

        return null;
    }

    public SituationNodeData GetNextSituation(string answerGUID)
    {
        foreach (var answer in NodeLinks)
        {
            if (answerGUID == answer.BaseNodeGuid)
            {
                var aux = GetSituation(answer.TargetNodeGuid);
                if (aux != null) //descartamos que sea un dialogueNode
                {
                    return aux;
                }
            }
        }

        return null;
    }

    public SituationNodeData GetSituation(string situationGUID)
    {
        foreach (var situation in SituationNodeData)
        {
            if (situation.Guid == situationGUID)
            {
                return situation;
            }
        }

        return null;
    }

    public DialogueNodeData GetNextDialogueData(string _GUID)
    {
        foreach (var node in NodeLinks) //node generico --> dialogue
        {
            if (_GUID == node.BaseNodeGuid)
            {
                var aux = GetDialogue(node.TargetNodeGuid);
                if (aux != null)
                {
                    return aux;
                }
            }
        }

        return null;
    }

    public DialogueNodeData GetDialogue(string _GUID)
    {
        foreach (var node in DialogueNodeData)
        {
            if (node.Guid == _GUID)
            {
                return node;
            }
        }

        return null;
    }

}
