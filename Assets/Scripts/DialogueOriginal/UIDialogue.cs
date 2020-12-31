using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The information about the different options the user can select and
/// its consecuences (UnityEvents).
/// This is a designer's tool for creating different dialogues quickly.
/// </summary>
public class UIDialogue : MonoBehaviour
{
    /// <summary>
    /// It saves the text to be displayed with this option and the
    /// UnityEvent that executes when the user selects it.
    /// </summary>
    [System.Serializable]
    public class DialogueOption
    {
        [TextArea]
        public string text;
        public UnityEvent OnSelected = new UnityEvent();
    }

    [SerializeField]
    // The dialogue director manages which dialogue is executing.
    // It works as a nexus.
    private UIDialogueDirector uiDialogueDirector;
    [SerializeField]
    // The position where the dialogue should be displayed.
    private Transform dialoguePositionTransform;
    public Transform DialoguePositionTransform
    {
        get
        {
            return dialoguePositionTransform;
        }

        private set
        {
            dialoguePositionTransform = value;
        }
    }
    [SerializeField]
    // The name of the character which dialogue options are displaying
    private string characterName;
    public string CharacterName
    {
        get
        {
            return characterName;
        }

        private set
        {
            characterName = value;
        }
    }
    [SerializeField]
    // Optional field for showing a brief description
    // of the conversation content.
    private string description;
    public string Description
    {
        get
        {
            return description;
        }

        private set
        {
            description = value;
        }
    }

    [SerializeField]
    // The different options this dialogue has.
    private List<DialogueOption> options;
    public List<DialogueOption> Options
    {
        get
        {
            return options;
        }

        private set
        {
            options = value;
        }
    }

    [ContextMenu("Play")]
    // This function gets the process of displaying the dialogue started.
    // It comunicates with the dialogue director.
    public void Play()
    {
        if (uiDialogueDirector)
        {
            uiDialogueDirector.Play(this);
        }
    }

    // When an option is selected, the consecuences of that action is executed.
    // Maybe this function shouldn't be here.
    public void OptionSelected(DialogueOption option)
    {
        option.OnSelected.Invoke();
    }
}
