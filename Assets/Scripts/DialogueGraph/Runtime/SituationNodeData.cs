using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SituationNodeData
{
    public string Guid;
    public string SituationName;
    public string Context;
    public string Id;
    public string audioId;
    public NodeType nodeType;
    //public bool EntryPoint = false;
    public Vector2 Position;
}
