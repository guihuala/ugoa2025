using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEncyclopediaPanel : BasePanel
{
    [SerializeField] private float pageSpeed = 0.5f;
    public List<EnemyData> enemies; // 存储所有敌人的数据
    public List<Transform> pages;   // 书本的页面

    public Button closeButton;
    public Button nextButton;  // 下一页按钮
    public Button previousButton; // 上一页按钮

    private int currentIndex = 0; // 当前显示的敌人索引
    private bool isRotating = false; // 旋转动画标记

    private void Start()
    {
        // 初始化页面信息
        InitializePages();
        
        closeButton.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        nextButton.onClick.AddListener(() => ShowNextEnemy());
        previousButton.onClick.AddListener(() => ShowPreviousEnemy());

        InitialState(); // 书本初始化
    }

    // 初始化书本状态
    private void InitialState()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].rotation = Quaternion.identity; // 确保所有页面初始状态
        }
        pages[0].SetAsLastSibling(); // 确保第一页在最上方
        previousButton.gameObject.SetActive(false);
    }

    // 初始化页面的信息
    private void InitializePages()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            DisplayEnemy(i);  // 加载所有页面的敌人信息
        }
    }

    // 显示当前敌人信息
    private void DisplayEnemy(int index)
    {
        if (index < 0 || index >= enemies.Count || index >= pages.Count)
            return;

        EnemyData enemy = enemies[index];
        Transform page = pages[index];

        Image pageEnemyImage = page.GetChild(0).GetChild(0).GetComponent<Image>();
        Text pageEnemyName = page.GetChild(0).GetChild(1).GetComponent<Text>();
        Text pageEnemyDescription = page.GetChild(0).GetChild(2).GetComponent<Text>();

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
            pageEnemyImage.sprite = enemy.enemySprite;
        }

        // 更新按钮状态
        previousButton.gameObject.SetActive(index > 0);
    }

    // 处理翻页动画
    private IEnumerator RotatePage(Transform page, float targetAngle, bool isForward)
    {
        isRotating = true;

        float duration = pageSpeed;
        float timeElapsed = 0f;

        Quaternion initialRotation = page.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(timeElapsed / duration);
            page.rotation = Quaternion.Slerp(initialRotation, targetRotation, progress);
            yield return null;
        }

        page.rotation = targetRotation;

        // 旋转结束后再更新页面和敌人信息
        if (isForward)
        {
            currentIndex++;
        }
        else
        {
            currentIndex--;
        }

        // 更新敌人信息
        DisplayEnemy(currentIndex);

        isRotating = false;
    }

    // 显示下一位敌人
    private void ShowNextEnemy()
    {
        if (isRotating || currentIndex >= enemies.Count - 1) return;

        Transform currentPage = pages[currentIndex];
        currentPage.SetAsLastSibling(); // 确保当前页显示在最上层
        StartCoroutine(RotatePage(currentPage, 180, true));
    }

    // 显示上一位敌人
    private void ShowPreviousEnemy()
    {
        if (isRotating || currentIndex <= 0) return;

        Transform currentPage = pages[currentIndex];
        StartCoroutine(RotatePage(currentPage, 0, false));
    }
}
