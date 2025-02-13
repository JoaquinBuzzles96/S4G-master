﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;


public abstract class ParentNode : Node
{
    public string GUID;
    public string nodeName;
    public NodeType nodeType;
    public string Description;
    public bool EntryPoint = false;
}
