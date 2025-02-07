using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour , IEnterSpecialItem
{
    private LevelInfo levelInfo;

    [SerializeField] private bool ifRequestCollection;
    private string[] requestedItemID = new[] { "7", "8", "9" };
    
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
                Debug.Log("All achievements requested");
            }
        }
        else
        {
            UIManager.Instance.OpenPanel("GameVictoryPanel"); 
        }
    }

}
