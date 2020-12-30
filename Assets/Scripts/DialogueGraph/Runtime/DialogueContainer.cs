using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class DialogueContainer : ScriptableObject
{
    [SerializeField]
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    [SerializeField]
    public List<DialogueNodeData> DialogueNodeData= new List<DialogueNodeData>();
}
