using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private DialogueData dialogueData;
    private bool isPlayed = false;

    public void Apply()
    {
        if (isPlayed)
            return;

        DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
        dialoguePanel.StartDialogue(dialogueData);
        
        isPlayed = true;
    }
}