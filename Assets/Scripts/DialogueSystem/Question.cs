using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class Question : ScriptableObject
{
    public string questionName;
    public string Description;
    //public Situation situation;
    public List<Answer> posibleAnswers;

}
