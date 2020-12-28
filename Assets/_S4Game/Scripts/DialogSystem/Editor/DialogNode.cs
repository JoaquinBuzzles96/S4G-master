using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogNode : Node
{
  public string GUID; // Unique ID for each node

  public string DialogText;

  public bool EntryPoint = false;
}
