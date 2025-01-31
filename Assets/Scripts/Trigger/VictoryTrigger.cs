using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour , IEnterSpecialItem
{
    private LevelInfo levelInfo;
    private void Start()
    {
        levelInfo = FindObjectOfType<LevelInfo>();
    }

    public void Apply()
    {
        UIManager.Instance.OpenPanel("GameVictoryPanel");
    }
}
