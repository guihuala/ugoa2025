using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour , IEnterSpecialItem
{
    private LevelInfo levelInfo;

    [SerializeField] private bool ifRequestCollection;
    private string[] requestedItemID = new[] { "7", "8", "9" };
    
    public DialogueData dialogueData;
    
    private void Start()
    {
        levelInfo = FindObjectOfType<LevelInfo>();
    }

    public void Apply()
    {
        if (ifRequestCollection)
        {
            HashSet<string> achievementList = AchievementManager.Instance.pendingAchievements;
            
            bool allRequestedItemsMet = true;

            foreach (var requestedItem in requestedItemID)
            {
                if (!achievementList.Contains(requestedItem))
                {
                    allRequestedItemsMet = false;
                    break;
                }
            }

            if (allRequestedItemsMet)
            {
                LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
                SaveManager.Instance.SetDefaultCurrentScene();
                levelInfo.VictorySaveLevel();
                
                SceneLoader.Instance.LoadScene(SceneName.CG,"...");
            }
            else
            {
                DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
                dialoguePanel.StartDialogue(dialogueData);
            }
        }
        else
        {
            UIManager.Instance.OpenPanel("GameVictoryPanel"); 
        }
    }
}
