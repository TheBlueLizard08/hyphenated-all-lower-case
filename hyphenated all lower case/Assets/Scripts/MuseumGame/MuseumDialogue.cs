using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MuseumDialogue : MonoBehaviour
{
    [Header("GameObject References")]
    public GameObject dialogueUIPanel;
    public AudioSource dialogueAudioSource;
    public TextMeshProUGUI dialogueTextUI, speakerNameUI, button0Text, button1Text;
    public Button button0, button1;
    public PlayerMovement museumPlayer;
    public MouseLook[] museumMouseLooks;

    [Header("Dialogue stuff")][TextArea]
    public List<string> dialogueText0;

    [TextArea]
    public List<string> replies0;
    public List<string> speakerNames0;
    public List<AudioClip> clips0;

    [TextArea]
    public string choiceDialogue;
    public string choiceAnswer0, choiceAnswer1, choiceSpeaker;
    public AudioClip choiceClip;

    [TextArea]
    public List<string> dialogueText1, replies1;
    public List<string> speakerNames1;
    public List<AudioClip> clips1;

    int lastAnswered = -1;

    public void PlayDialogue()
    {
        StartCoroutine(PlayDialogueC());
    }

    IEnumerator PlayDialogueC()
    {
        //Initialize dialogue
        dialogueUIPanel.SetActive(true);
        button0.gameObject.SetActive(true);
        button1.gameObject.SetActive(false);
        museumPlayer.enabled = false;
        foreach(MouseLook m in museumMouseLooks) { m.enabled = false; }
        Cursor.lockState = CursorLockMode.Confined;

        //Play first bit of dialogue
        for (int i = 0; i < dialogueText0.Count; i++)
        {
            lastAnswered = -1;
            dialogueAudioSource.Stop();

            dialogueTextUI.text = dialogueText0[i];
            speakerNameUI.text = speakerNames0[i];
            button0Text.text = replies0[i];

            dialogueAudioSource.clip = clips0[i];
            dialogueAudioSource.Play();

            while(lastAnswered == -1) { yield return null; }
        }

        //Play the choice dialogue (god this is ugly)
        lastAnswered = -1;
        dialogueAudioSource.Stop();

        dialogueTextUI.text = choiceDialogue;
        speakerNameUI.text = choiceSpeaker;
        button0Text.text = choiceAnswer0;
        button1Text.text = choiceAnswer1;
        button1.gameObject.SetActive(true);

        dialogueAudioSource.clip = choiceClip;
        dialogueAudioSource.Play();

        while(lastAnswered == -1) { yield return null; }

        if(lastAnswered == 1) { CloseDialogue(); yield break; } //End dialogue if the player chooses option 1, otherwise continue

        button1.gameObject.SetActive(false);

        //Duplicated code go BRRRRRRRRRRRRRRRRRRRRRRR
        //We ignore the voices in my head screaming that this is terrible :)
        //We also ignore the MSO professor knocking on my door while holding a knife in a threatening manner
        //sorry Hans (I think his name was Hans, I forgot his last name)
        for (int i = 0; i < dialogueText1.Count; i++)
        {
            lastAnswered = -1;
            dialogueAudioSource.Stop();

            dialogueTextUI.text = dialogueText1[i];
            speakerNameUI.text = speakerNames1[i];
            button0Text.text = replies1[i];

            dialogueAudioSource.clip = clips1[i];
            dialogueAudioSource.Play();

            while (lastAnswered == -1) { yield return null; }
        }

        CloseDialogue();
    }

    void CloseDialogue()
    {
        dialogueUIPanel.SetActive(false);
        dialogueAudioSource.Stop();
        museumPlayer.enabled = true;
        foreach (MouseLook m in museumMouseLooks) { m.enabled = true; }
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PressAnswer(int answerIndex)
    {
        if(lastAnswered != -1) { return; }
        lastAnswered = answerIndex;
    }
}
