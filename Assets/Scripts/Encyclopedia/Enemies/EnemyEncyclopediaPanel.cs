using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class EnemyEncyclopediaPanel : BasePanel
{
    public List<EnemyData> enemies; // 存储所有敌人的数据
    public List<Transform> pages;   // 书本的页面
    [FormerlySerializedAs("bookController")] public BookController bookControllerController;     // 书本控制器

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

        bookControllerController.InitialState(); // 书本初始化
    }

    // 显示当前敌人信息
    void DisplayEnemy(int index)
    {
        if (index < 0 || index >= enemies.Count || index >= pages.Count)
            return;

        EnemyData enemy = enemies[index];

        Transform page = pages[index]; // 获取当前翻到的页面
        Image pageEnemyImage = page.Find("EnemyImage").GetComponent<Image>();
        Text pageEnemyName = page.Find("EnemyName").GetComponent<Text>();
        Text pageEnemyDescription = page.Find("EnemyDescription").GetComponent<Text>();

        if (enemy.isUnlocked)
        {
            pageEnemyName.text = enemy.enemyName;
            pageEnemyDescription.text = enemy.enemyDescription;
            pageEnemyImage.sprite = enemy.enemySprite;
        }
        else
        {
            pageEnemyName.text = "???";
            pageEnemyDescription.text = "尚未解锁";
            pageEnemyImage.sprite = null;
        }
    }

    void UpdateEnemyState()
    {
        foreach (var enemy in enemies)
        {
            enemy.CheckUnlock(LevelManager.Instance.levels);
        }
    }

    // 显示下一位敌人（模拟翻页）
    void ShowNextEnemy()
    {
        if (currentIndex < enemies.Count - 1)
        {
            currentIndex++;
            bookControllerController.RotateForward();
            DisplayEnemy(currentIndex);
        }
    }

    // 显示上一位敌人（模拟翻页）
    void ShowPreviousEnemy()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            bookControllerController.RotateBack();
            DisplayEnemy(currentIndex);
        }
    }
}
