using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineDialogueDirector : MonoBehaviour
{
    private PlayableDirector director;
    private TimelineDialogue currentTimelineDialogue;

    private void OnEnable()
    {
        director = GetComponent<PlayableDirector>();

        if(director)
        {
            director.played += OnBegin;
            director.stopped += OnStop;
        }

        currentTimelineDialogue = null;
    }

    private void OnDisable()
    {
        if (director)
        {
            director.played -= OnBegin;
            director.stopped -= OnStop;
        }
    }

    public void Play(TimelineDialogue timelineDialogue)
    {
        if(currentTimelineDialogue != null)
        {
            director.Stop();
        }

        currentTimelineDialogue = timelineDialogue;
        director.Play(timelineDialogue.GetTimeline());
    }

    private void OnBegin(PlayableDirector director)
    {
        if(currentTimelineDialogue)
        {
            currentTimelineDialogue.OnBegin();
        }
    }

    private void OnStop(PlayableDirector director)
    {
        if (currentTimelineDialogue)
        {
            currentTimelineDialogue.OnEnd();
            currentTimelineDialogue = null;
        }
    }
}
