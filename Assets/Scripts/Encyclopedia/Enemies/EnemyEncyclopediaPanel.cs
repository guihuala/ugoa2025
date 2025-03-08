using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEncyclopediaPanel : BasePanel
{
    public List<EnemyData> enemies; // 存储所有敌人的数据

    public Image enemySpriteImg;
    public Text enemyNameText;
    public Text enemyDescriptionText;

    public Button closeButton;
    public Button nextButton; // 下一页按钮
    public Button previousButton; // 上一页按钮

    private int currentIndex = 0; // 当前显示的敌人索引

    private void Start()
    {
        UpdateEnemyState();
        DisplayEnemy(currentIndex);

        closeButton.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        nextButton.onClick.AddListener(() => ShowNextEnemy());
        previousButton.onClick.AddListener(() => ShowPreviousEnemy());
    }

    // 显示当前敌人信息
    void DisplayEnemy(int index)
    {
        if (index < 0 || index >= enemies.Count)
            return;
        
        EnemyData enemy = enemies[index];

        if (enemy.isUnlocked)
        {
            enemyNameText.text = enemy.enemyName;
            enemyDescriptionText.text = enemy.enemyDescription;
            enemySpriteImg.sprite = enemy.enemySprite;
        }
        else
        {
            enemyNameText.text = "lock";
            enemyDescriptionText.text = "";
            enemySpriteImg.sprite = enemy.enemySprite;
        }
    }

    void UpdateEnemyState()
    {
        foreach (var enemy in enemies)
        {
            enemy.CheckUnlock(LevelManager.Instance.levels);
        }
    }

    // 显示下一位敌人
    void ShowNextEnemy()
    {
        currentIndex++;
        if (currentIndex >= enemies.Count)
            currentIndex = 0;  // 如果超出敌人数量，则回到第一个敌人

        DisplayEnemy(currentIndex);
    }

    // 显示上一位敌人
    void ShowPreviousEnemy()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = enemies.Count - 1;  // 如果超出范围，则回到最后一个敌人

        DisplayEnemy(currentIndex);
    }
}
