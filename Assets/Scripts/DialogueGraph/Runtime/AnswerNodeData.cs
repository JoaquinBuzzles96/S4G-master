using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//public enum Speaker { Surgeon1, Surgeon2, CirculatingNurse, AnaesthesiaNurse, InstrumentalistNurse, Anaesthesiologist};

[Serializable]
public class AnswerNodeData
{
    public string Guid;
    public string AnswerName;
    public string Feedback;
    public string Situation;
    public bool IsCorrect;
    public bool IsEnd;
    public NodeType nodeType;
    public string speaker;
    public string audioId;
    public int score;
    public Vector2 Position;
}
