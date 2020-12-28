using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueCanvas : MonoBehaviour
{
    [SerializeField]
    // When the dialogue option is selected, we tell the
    // dialogue director.
    private UIDialogueDirector dialogueDirector;
    [SerializeField]
    // The main GameObject of the whole UI display.
    private GameObject dialogueParent;
    [SerializeField]
    // The UI text that represents the character name.
    private TextMeshProUGUI characterNameText;
    [SerializeField]
    // The UI text that represents the optional description.
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    // The parent of all the dialogues UI texts.
    // It works by using a GridLayoutGroup.
    private Transform optionsParent;

    /// <summary>
    /// A class containing all the posible information needed
    /// for the correct display of a dialogue option.
    /// It also tells us when the button has been clicked.
    /// </summary>
    private class DialogueOptionUI
    {
        public GameObject gameObject;
        public TextMeshProUGUI text;
        public Button button;

        private DialogueCanvas dialogueCanvas;

        public DialogueOptionUI(GameObject gameObject, TextMeshProUGUI text, Button button, DialogueCanvas dialogueCanvas)
        {
            this.gameObject = gameObject;
            this.text = text;
            this.button = button;
            this.dialogueCanvas = dialogueCanvas;
            this.button.onClick.AddListener(OnSelected);
        }

        private void OnSelected()
        {
            dialogueCanvas.OptionSelected(this);
        }
    }

    // A list of all the UI options for the user to select.
    private List<DialogueOptionUI> dialogueOptionsUI;

    // The current information of the dialogue.
    private UIDialogue currentUIDialogue;

    private void Start()
    {
        InitializePossibleUIOptions();
    }

    private void InitializePossibleUIOptions()
    {
        dialogueOptionsUI = new List<DialogueOptionUI>();

        int optionsParentChildCount = optionsParent.childCount;

        for (int i = 0; i < optionsParentChildCount; i++)
        {
            Transform child = optionsParent.GetChild(i);
            CreateDialogueOptionUIFromUIObjectComponents(child);
        }
    }

    private void CreateDialogueOptionUIFromUIObjectComponents(Transform UIObject)
    {
        DialogueOptionUI dialogueOptionUI = new DialogueOptionUI(UIObject.gameObject, UIObject.GetComponent<TextMeshProUGUI>(), UIObject.GetComponent<Button>(), this);
        dialogueOptionsUI.Add(dialogueOptionUI);
    }

    public void RepresentUIDialogue(UIDialogue uiDialogue)
    {
        currentUIDialogue = uiDialogue;
        dialogueParent.SetActive(true);
        SetCharacterNameAndDescription(uiDialogue.CharacterName, uiDialogue.Description);
        DisplayNeededUIOptions(currentUIDialogue.Options);
    }

    private void SetCharacterNameAndDescription(string name, string description)
    {
        characterNameText.text = name;
        if (!string.IsNullOrEmpty(description))
        {
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = description;
        }
        else
        {
            descriptionText.gameObject.SetActive(false);
        }
    }

    private void DisplayNeededUIOptions(List<UIDialogue.DialogueOption> dialogueOptions)
    {
        int maxNumberOfOptions = dialogueOptionsUI.Count;
        int numberOfOptions = dialogueOptions.Count;
        for (int i = 0; i < maxNumberOfOptions; i++)
        {
            if (i < numberOfOptions)
            {
                dialogueOptionsUI[i].gameObject.SetActive(true);
                dialogueOptionsUI[i].text.text = dialogueOptions[i].text;
            }
            else
            {
                dialogueOptionsUI[i].gameObject.SetActive(false);
            }
        }
    }

    private void OptionSelected(DialogueOptionUI dialogueOptionUI)
    {
        int optionIndexSelected = dialogueOptionsUI.IndexOf(dialogueOptionUI);
        dialogueDirector.OptionSelected(currentUIDialogue.Options[optionIndexSelected]);
        currentUIDialogue = null;
        dialogueParent.SetActive(false);
    }
}
