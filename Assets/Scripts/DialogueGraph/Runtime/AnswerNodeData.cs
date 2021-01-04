using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnswerNodeData
{
    public string Guid;
    public string AnswerName;
    public string Description;
    public string Situation;
    public bool IsCorrect;
    public bool IsEnd;
    public NodeType nodeType;
    public Vector2 Position;
}
