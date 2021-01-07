using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogueContainer : ScriptableObject
{

    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    //public List<ParentNodeData> ParentNodeData= new List<ParentNodeData>();
    public List<SituationNodeData> SituationNodeData= new List<SituationNodeData>();
    public List<QuestionNodeData> QuestionNodeData= new List<QuestionNodeData>();
    public List<AnswerNodeData> AnswerNodeData= new List<AnswerNodeData>();

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
            if (situationGUID == situation.BaseNodeGuid)
            {
                listAux.Add(GetQuestion(situation.TargetNodeGuid));
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
                return GetSituation(answer.TargetNodeGuid);
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

}
