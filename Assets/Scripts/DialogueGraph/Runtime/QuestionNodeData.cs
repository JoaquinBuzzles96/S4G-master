using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuestionNodeData
{
    public string Guid;
    public string QuestionName;
    public string Description;
    public NodeType nodeType;
    public Vector2 Position;
}
