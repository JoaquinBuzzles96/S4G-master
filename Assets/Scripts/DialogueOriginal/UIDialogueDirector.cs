using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// UIDialogueDirector is a nexus between the dialogue information, events,
/// and the UI that displays the information and gets the player input to select an
/// option.
/// </summary>
public class UIDialogueDirector : MonoBehaviour
{
    [SerializeField]
    // The UI that displays the dialogue options.
    private DialogueCanvas dialogueCanvas;

    // The current dialogue that is taking course.
    // It saves information about the different options
    // the user can select, and its consecuences (UnityEvents).
    private UIDialogue currentUIDialogue;

    // This function allows a dialogue to be played, connecting
    // the information to the UI.
    public void Play(UIDialogue uiDialogue)
    {
        currentUIDialogue = uiDialogue;
        dialogueCanvas.RepresentUIDialogue(currentUIDialogue);
    }

    // When an option is selected, we execute the given consecuences (UnityEvents).
    public void OptionSelected(UIDialogue.DialogueOption dialogueOption)
    {
        currentUIDialogue.OptionSelected(dialogueOption);
        currentUIDialogue = null;
    }
}
