using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class AnswerNode : ParentNode
{
    public string Situation;

    public bool IsCorrect;

    public bool IsEnd;

    public string speaker;

    public string audioId;

    public int score;
}
