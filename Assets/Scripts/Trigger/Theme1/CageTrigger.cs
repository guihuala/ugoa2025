using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private string[] requestedItemID;
    public DialogueData dialogueData;
    
    private LockedCageBehavior lockedCage;


    private void Start()
    {
        lockedCage = FindObjectOfType<LockedCageBehavior>();
    }

    public void Apply()
    {
        HashSet<string> achievementList = AchievementManager.Instance.pendingAchievements;

        bool allRequestedItemsMet = true;

        foreach (var requestedItem in requestedItemID)
        {
            if (!achievementList.Contains(requestedItem))
            {
                Debug.Log(requestedItem);
                
                allRequestedItemsMet = false;
                break;
            }
        }

        if (allRequestedItemsMet) // 收集完成，解锁鸟笼
        {
            lockedCage.UnlockCage();
        }
        else // 未收集完的情况
        {
            DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
            dialoguePanel.StartDialogue(dialogueData);
        }
    }
}