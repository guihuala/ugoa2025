using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialoguePanel : BasePanel
{
    #region 对话面板

    private Image _characterImage;
    private Text _characterNameText;
    private Text _contentText;
    private Button _skipBtn;

    #endregion

    public SelectPanel selectPanel;

    private DialogueData _currentData;  // 当前对话数据
    private int _currentIndex;          // 当前对话索引
    private bool _isWaitingForSelect;   // 是否在等待用户选择
    private bool _isTyping;             // 是否正在播放打字机效果
    private Tween _typingTween;         // DOTween 打字机效果动画
    
    private bool _isDialogueEnding;     // 对话是否已到末尾

    protected override void Awake()
    {
        base.Awake();

        _characterNameText = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>();
        _contentText = transform.GetChild(1).GetChild(1).GetComponent<Text>();
        _characterImage = transform.GetChild(0).GetComponent<Image>();

        _skipBtn = transform.GetChild(2).GetComponent<Button>();
        _skipBtn.onClick.AddListener(SkipAllDialogue);
    }
    
    public override void OpenPanel(string name)
    {
        base.OpenPanel(name); // 调用基类的打开面板方法
        
        DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                Time.timeScale = 0; // 暂停游戏
            });
    }

    public override void ClosePanel()
    {
        Time.timeScale = 1; // 恢复游戏速度
        
        base.ClosePanel();
    }

    public void StartDialogue(DialogueData data)
    {
        _currentData = data;
        _currentIndex = 0;
        _isDialogueEnding = false;

        RefreshDialogue();
    }

    private void RefreshDialogue()
    {
        if (_currentData == null || _currentData.Cells.Count == 0)
        {
            Debug.LogError("DialogueBlock is empty or null!");
            return;
        }

        var currentCell = _currentData.Cells[_currentIndex];

        // 更新角色头像
        if (currentCell.CharacterSprite != null)
        {
            _characterImage.sprite = currentCell.CharacterSprite;
            _characterImage.enabled = true;
        }
        else
        {
            _characterImage.enabled = false;
        }

        // 更新角色名字
        _characterNameText.text = currentCell.NPC != null ? currentCell.NPC.NPCName : currentCell.CharacterName;

        // 打字机效果（忽略时间缩放）
        _typingTween?.Kill();
        _isTyping = true;
        _contentText.text = ""; // 清空文本

        _typingTween = _contentText.DOText(currentCell.Content, currentCell.Content.Length * 0.05f)
            .SetEase(Ease.Linear)
            .SetUpdate(true) // 忽略时间缩放
            .OnComplete(() =>
            {
                _isTyping = false;
            });
    }

    public void NextDialogue()
    {
        if (_currentData == null || _currentIndex >= _currentData.Cells.Count - 1)
        {
            Debug.LogWarning("No more dialogue available!");
            return;
        }

        // 检查是否为分支选择
        if (_currentData.Cells[_currentIndex + 1].CellType == CellType.Select)
        {
            _isWaitingForSelect = true;
            selectPanel.gameObject.SetActive(true);

            int tempIndex = _currentIndex + 1;
            while (tempIndex < _currentData.Cells.Count && _currentData.Cells[tempIndex].CellType == CellType.Select)
            {
                selectPanel.AddCell(_currentData.Cells[tempIndex], this);
                tempIndex++;
            }
        }
        else
        {
            _currentIndex++;
        }

        // 检查是否为最后一条对话
        if (_currentData.Cells[_currentIndex].CellFlag == CellFlag.End)
        {
            _isDialogueEnding = true;
        }
    }

    public void OnPointerDown()
    {
        if (_isWaitingForSelect) return;

        if (_isTyping)
        {
            // 如果正在打字，则直接完成打字效果
            _typingTween?.Complete(true);
        }
        else
        {
            if (_isDialogueEnding)
            {
                EndDialogue();
            }
            else
            {
                NextDialogue();
                RefreshDialogue();
            }
        }
    }

    private void EndDialogue()
    {
        UIManager.Instance.ClosePanel(panelName);
    }

    public void SetSelectDialogue(int jumpToIndex)
    {
        _isWaitingForSelect = false;
        _currentIndex = jumpToIndex - 1;

        NextDialogue();
        RefreshDialogue();
        selectPanel.gameObject.SetActive(false);
    }

    public void SkipAllDialogue()
    {
        _typingTween?.Kill(); // 停止打字机效果
        _isTyping = false;

        if (_currentData == null) return;

        // 跳转到最后一条对话
        _currentIndex = _currentData.Cells.Count - 1;

        var finalCell = _currentData.Cells[_currentIndex];
        _characterImage.sprite = finalCell.CharacterSprite;
        _characterNameText.text = finalCell.CharacterName;
        _contentText.text = finalCell.Content;

        _isDialogueEnding = true;

        EndDialogue();
    }
}
