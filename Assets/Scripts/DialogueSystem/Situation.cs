using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Situation", menuName = "Situation")]
public class Situation : ScriptableObject
{
    public string situationName;
    public string description;
    public int id;
    public List<Question> questions = new List<Question>();

    public Question question1;
    public Question question2;
    public Question question3;
    public Question question4;

    public int iterator = 0;
}
