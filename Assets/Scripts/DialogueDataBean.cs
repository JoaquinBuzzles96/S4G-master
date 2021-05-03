using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDataBean
{

    public string name; //id
    public string speaker;
    public string text;

    public DialogueDataBean(string name, string speaker, string text)
    {
        this.name = name;
        this.speaker = speaker;
        this.text = text;
    }

}
