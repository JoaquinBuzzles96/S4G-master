using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TimelineDialogue : MonoBehaviour
{
    [SerializeField]
    private TimelineDialogueDirector timelineDialogueDirector;
    [SerializeField]
    private PlayableAsset timeline;

    public UnityEvent OnBeginDialogue = new UnityEvent();
    public UnityEvent OnEndDialogue = new UnityEvent();

    [ContextMenu("Play")]
    public void Play()
    {
        if(timelineDialogueDirector)
        {
            timelineDialogueDirector.Play(this);
        }
    }

    public void OnBegin()
    {
        OnBeginDialogue.Invoke();
    }

    public void OnEnd()
    {
        OnEndDialogue.Invoke();
    }

    public PlayableAsset GetTimeline()
    {
        return timeline;
    }
}
