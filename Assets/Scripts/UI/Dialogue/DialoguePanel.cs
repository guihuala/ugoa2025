using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialoguePanel: BasePanel
{

    #region �Ի����

    private Image _characterImage;
    private Text _characterNameText;
    private Text _contentText;
    private Button _skipBtn;

    #endregion

    public SelectPanel selectPanel;

    private DialogueBlock _currentBlock;  // ��ǰ�Ի���
    private int _currentIndex;           // ��ǰ�Ի�����
    private bool _isWaitingForSelect;    // �Ƿ��ڵȴ��û�ѡ��
    private bool _isTyping;              // �Ƿ����ڲ��Ŵ��ֻ�����
    private Tween _typingTween;          // DOTween ���ֻ���������
    
    private bool _isDialogueEnding;               // �Ի��Ƿ��ѵ�ĩβ

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

        // ���½�ɫͷ��
        if (currentCell.CharacterSprite != null)
        {
            _characterImage.sprite = currentCell.CharacterSprite;
            _characterImage.enabled = true;
        }
        else
        {
            _characterImage.enabled = false;
        }

        // ���½�ɫ����
        _characterNameText.text = currentCell.NPC != null ? currentCell.NPC.NPCName : currentCell.CharacterName;

        // ���ֻ�Ч��
        _typingTween?.Kill();
        _isTyping = true;
        _contentText.text = ""; // �������

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

        // ����Ƿ�Ϊ��֧ѡ��
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

        // ����Ƿ�Ϊ���һ�ζԻ�
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
            // ������ڴ��֣�������ɴ��ֶ���
            _typingTween?.Complete();
        }
        else
        {
            // ����Ի�����ɣ�������һ��
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
        _typingTween?.Kill(); // ֹͣ���ֶ���
        _isTyping = false;

        if (_currentBlock == null) return;

        // �������һ�ζԻ�
        _currentIndex = _currentBlock.Cells.Count - 1;

        var finalCell = _currentBlock.Cells[_currentIndex];
        _characterImage.sprite = finalCell.CharacterSprite;
        _characterNameText.text = finalCell.CharacterName;
        _contentText.text = finalCell.Content;

        _isDialogueEnding = true;

        EndDialogue();
    }
    
}