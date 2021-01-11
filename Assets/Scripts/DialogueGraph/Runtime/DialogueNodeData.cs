using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogueNodeData
{
    public string Guid;
    public string DialogueName;
    public string DialogueText;
    public string Speaker;
    public string Mood;
    public string audioId;
    public NodeType nodeType;
    public Vector2 Position;
}
