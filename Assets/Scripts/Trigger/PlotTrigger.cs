using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotTrigger : MonoBehaviour, IEnterSpecialItem
{
    public DialogueData dialogueData;
    private GameObject particleObject;
    
    private void Start()
    {
        particleObject = transform.GetChild(3).gameObject;
    }

    public void Apply()
    {
        if (particleObject != null)
        {
            DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
            dialoguePanel.StartDialogue(dialogueData);
            
            Destroy(particleObject);
        }
    }
}