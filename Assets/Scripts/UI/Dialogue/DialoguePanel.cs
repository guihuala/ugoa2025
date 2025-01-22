using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialoguePanel: BasePanel
{

    #region 对话组件

    private Image _characterImage;
    private Text _characterNameText;
    private Text _contentText;
    private Button _skipBtn;

    #endregion

    public SelectPanel selectPanel;

    private DialogueBlock _currentBlock;  // 当前对话块
    private int _currentIndex;           // 当前对话索引
    private bool _isWaitingForSelect;    // 是否在等待用户选择
    private bool _isTyping;              // 是否正在播放打字机动画
    private Tween _typingTween;          // DOTween 打字机动画对象
    
    private bool _isDialogueEnding;               // 对话是否已到末尾

    protected override void Awake()
    {
        base.Awake();

        _characterNameText = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>();
        _contentText = transform.GetChild(1).GetChild(1).GetComponent<Text>();
        _characterImage = transform.GetChild(0).GetComponent<Image>();

        _skipBtn = transform.GetChild(2).GetComponent<Button>();
        _skipBtn.onClick.AddListener(SkipAllDialogue);

    }
public void StartDialogue(DialogueBlock block)
    {
        _currentBlock = block;
        _currentIndex = 0;
        _isDialogueEnding = false;

        RefreshDialogue();
    }

    private void RefreshDialogue()
    {
        if (_currentBlock == null || _currentBlock.Cells.Count == 0)
        {
            Debug.LogError("DialogueBlock is empty or null!");
            return;
        }

        var currentCell = _currentBlock.Cells[_currentIndex];

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

        // 更新角色名称
        _characterNameText.text = currentCell.NPC != null ? currentCell.NPC.NPCName : currentCell.CharacterName;

        // 打字机效果
        _typingTween?.Kill();
        _isTyping = true;
        _contentText.text = ""; // 清空内容

        _typingTween = _contentText.DOText(currentCell.Content, currentCell.Content.Length * 0.05f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isTyping = false;
            });
    }


    private void NextDialogue()
    {
        if (_currentBlock == null || _currentIndex >= _currentBlock.Cells.Count - 1)
        {
            Debug.LogWarning("No more dialogue available!");
            return;
        }

        // 检查是否为分支选择
        if (_currentBlock.Cells[_currentIndex + 1].CellType == CellType.Select)
        {
            _isWaitingForSelect = true;
            selectPanel.gameObject.SetActive(true);

            int tempIndex = _currentIndex + 1;
            while (tempIndex < _currentBlock.Cells.Count && _currentBlock.Cells[tempIndex].CellType == CellType.Select)
            {
                selectPanel.AddCell(_currentBlock.Cells[tempIndex], this);
                tempIndex++;
            }
        }
        else
        {
            _currentIndex++;
        }

        // 检查是否为最后一段对话
        if (_currentBlock.Cells[_currentIndex].CellFlag == CellFlag.End)
        {
            _isDialogueEnding = true;
        }
    }

    public void OnPointerDown()
    {
        if (_isWaitingForSelect) return;

        if (_isTyping)
        {
            // 如果正在打字，立即完成打字动画
            _typingTween?.Complete();
        }
        else
        {
            // 如果对话已完成，继续下一段
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
        _typingTween?.Kill(); // 停止打字动画
        _isTyping = false;

        if (_currentBlock == null) return;

        // 跳到最后一段对话
        _currentIndex = _currentBlock.Cells.Count - 1;

        var finalCell = _currentBlock.Cells[_currentIndex];
        _characterImage.sprite = finalCell.CharacterSprite;
        _characterNameText.text = finalCell.CharacterName;
        _contentText.text = finalCell.Content;

        _isDialogueEnding = true;

        EndDialogue();
    }
    
}