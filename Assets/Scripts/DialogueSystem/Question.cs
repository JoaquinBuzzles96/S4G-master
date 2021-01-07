using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class Question : ScriptableObject
{
    public string questionName;
    public string Description;
    //public Situation situation;
    public List<Answer> posibleAnswers = new List<Answer>();

    public Answer answer1;
    public Answer answer2;
    public Answer answer3;
    public Answer answer4;

    public int iterator = 0;

}
