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
}
