using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI receptionText;
    public TextMeshProUGUI titleText;
    public AudioSource audioSource;

    [ContextMenu("Play")]
    public void Play()
    {
        //Esto se esta invocando al pisar el trigger de la recepcion
        receptionText.text = "";

        switch (LanguageManager.Instance.languageSelected)
        {
            case "EN":
                receptionText.text = "Good morning, please proceed to the operating room.";
                titleText.text = "INSTRUCTIONS";
                PlayAudio("EN");
                //TODO: Play del audio
                break;
            case "ES":
                receptionText.text = "Buenos días, por favor dirijase a la sala de cirugía.";
                titleText.text = "INSTRUCCIONES";
                PlayAudio("ES");
                break;
            case "PT":
                receptionText.text = "Bom dia, dirijam-se por favor para a sala de operações.";
                titleText.text = "INSTRUÇÕES";
                PlayAudio("PT");
                break;
            case "HU":
                receptionText.text = "Jó reggelt, kérem, menjenek a műtőbe.";
                titleText.text = "ÚTMUTATÓ";
                PlayAudio("HU");
                break;
            case "CZ":
                receptionText.text = "Dobrý den, prosím, pokračujte na operační sál.";
                titleText.text = "INSTRUKCE";
                PlayAudio("CZ");
                break;
        }

        /*
        if (timelineDialogueDirector)
        {
            timelineDialogueDirector.Play(this);
        }
        */
    }

    public void PlayAudio(string languague)
    {
        //Audio //Tutorial_PT //$"Audio/Case5_EN/Audio1
        string path = $"Audio/Tutorial/Tutorial_{languague}";

        audioSource.clip = Resources.Load(path, typeof(AudioClip)) as AudioClip;


        if (audioSource.clip != null)
        {
            audioSource.Play();
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
