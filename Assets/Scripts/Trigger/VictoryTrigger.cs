using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour , IEnterSpecialItem
{
    private LevelInfo levelInfo;

    [SerializeField] private bool ifRequestCollection;
    [SerializeField] private string[] requestedItemID;
    
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
                SaveManager.Instance.SetDefaultCurrentScene();
                levelInfo.VictorySaveLevel();

                if (levelInfo.isEndLevel == true)// 需要是最后一关，才会进入CG
                {
                    SaveManager.Instance.NewRecord();
                    SceneLoader.Instance.LoadScene(SceneName.CG,"...");
                }
                else// 否则打开画布
                {
                    levelInfo.VictorySaveLevel();
            
                    SaveManager.Instance.NewRecord();
                    UIManager.Instance.OpenPanel("GameVictoryPanel"); 
                }
            }
            else
            {
                DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
                dialoguePanel.StartDialogue(dialogueData);
            }
        }
        else
        {
            levelInfo.VictorySaveLevel();
            
            SaveManager.Instance.NewRecord();
            UIManager.Instance.OpenPanel("GameVictoryPanel"); 
        }
    }
}
