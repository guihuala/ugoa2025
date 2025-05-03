using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class MailboxPanel : BasePanel
{
    [Header("UI组件")]
    public Transform letterTransform;   // 信件Transform
    public Button letterButton; // 信件按钮
    public Text missionTitle; // 任务标题
    public Text missionDescriptionText; // 任务描述文本
    public Button closePanelBtn;

    [Header("动画设置")]
    public float letterPopDuration = 0.7f;
    public float letterPopHeight = 1.5f;
    public float letterCloseDuration = 0.5f;
    public Ease popEase = Ease.OutBack;
    public Ease closeEase = Ease.InBack;

    private Vector3 letterOriginalPos;
    private Sequence letterSequence;
    private MailboxNews mailboxNews;

    // 任务相关变量
    private Queue<MissionData> pendingMissions = new Queue<MissionData>();
    private MissionData currentMission;
    private bool isOpening = false;
    private bool isProcessingMissions = false;

    protected override void Awake()
    {
        base.Awake();
        
        // 保存信件原始位置
        letterOriginalPos = letterTransform.localPosition;
        
        // 初始化UI状态
        letterTransform.gameObject.SetActive(false);
        closePanelBtn.interactable = true;
        
        // 缓存MailboxNews引用
        mailboxNews = FindObjectOfType<MailboxNews>(true);
    }

    private void Start()
    {
        letterButton.onClick.AddListener(HideMissionLetter);
        closePanelBtn.onClick.AddListener(OnCloseMailbox);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
        // 重置状态
        isProcessingMissions = true;
        
        // 初始化待处理任务队列
        InitPendingMissions();
        
        // 开始处理任务
        ProcessNextMission();
    }

    // 初始化待处理任务队列
    private void InitPendingMissions()
    {
        pendingMissions.Clear();

        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("LevelManager instance not found!");
            return;
        }

        // 获取所有已解锁但未接受的任务
        foreach (var mission in LevelManager.Instance.missions)
        {
            if (mission != null && mission.isMissionUnlocked && !mission.isMissionAccepted)
            {
                pendingMissions.Enqueue(mission);
            }
        }
    }

    // 处理下一个任务
    private void ProcessNextMission()
    {
        if (isOpening || !isProcessingMissions) return;

        // 如果没有打开的信件且有任务待处理
        if (pendingMissions.Count > 0)
        {
            currentMission = pendingMissions.Dequeue();
            if (currentMission != null)
            {
                ShowMissionLetter(currentMission);
            }
            else
            {
                // 如果任务无效，继续处理下一个
                ProcessNextMission();
            }
        }
        else
        {
            // 没有更多任务时完成处理
            isProcessingMissions = false;
            
            // 更新MailboxNews的状态
            if (mailboxNews != null)
            {
                mailboxNews.HasPendingMissions();
            }
            
            // 关闭邮箱
            UIManager.Instance.ClosePanel(panelName);
        }
    }

    // 显示任务信件
    private void ShowMissionLetter(MissionData mission)
    {
        if (mission == null || isOpening) return;

        isOpening = true;
        closePanelBtn.interactable = false;

        missionTitle.text = mission.missionTitle;
        missionDescriptionText.text = mission.missionDescription;

        // 重置信件位置和大小
        letterTransform.localPosition = letterOriginalPos;
        letterTransform.localScale = Vector3.zero;
        letterTransform.gameObject.SetActive(true);
        
        // 停止之前的动画
        if (letterSequence != null && letterSequence.IsActive())
        {
            letterSequence.Kill();
            letterSequence = null;
        }

        // 播放信件弹出动画
        OpenLetterAnimation();
    }

    private void HideMissionLetter()
    {
        if (currentMission == null || !isOpening) return;

        CloseLetterAnimation();
    }

    // 播放信件打开动画
    private void OpenLetterAnimation()
    {
        letterSequence = DOTween.Sequence();

        // 弹出动画
        letterSequence.Append(letterTransform.DOScale(1, letterPopDuration * 0.5f).SetEase(popEase));
        letterSequence.Join(letterTransform.DOLocalMoveY(letterOriginalPos.y + letterPopHeight, letterPopDuration).SetEase(Ease.OutQuad));

        // 轻微晃动
        letterSequence.Append(letterTransform.DOShakeRotation(0.3f, new Vector3(0, 0, 15), 10, 50));

        letterSequence.OnComplete(() =>
        {
            // 确保信件保持在最终位置
            letterTransform.localPosition = new Vector3(
                letterTransform.localPosition.x,
                letterOriginalPos.y + letterPopHeight,
                letterTransform.localPosition.z
            );
            
            // 允许关闭面板
            closePanelBtn.interactable = true;
        });
    }

    // 播放信件关闭动画
    private void CloseLetterAnimation()
    {
        // 防止重复点击
        if (!isOpening) return;
        
        // 禁用交互
        closePanelBtn.interactable = false;
        letterButton.interactable = false;

        // 停止之前的动画
        if (letterSequence != null && letterSequence.IsActive())
        {
            letterSequence.Kill();
        }

        letterSequence = DOTween.Sequence();

        // 关闭动画 - 下落并缩小
        letterSequence.Append(letterTransform.DOLocalMoveY(letterOriginalPos.y, letterCloseDuration).SetEase(closeEase));
        letterSequence.Join(letterTransform.DOScale(0, letterCloseDuration).SetEase(closeEase));
        
        letterSequence.OnComplete(() =>
        {
            // 接受当前显示的任务
            if (LevelManager.Instance != null && currentMission != null)
            {
                LevelManager.Instance.AcceptMission(currentMission.missionID);
            }

            // 重置状态
            currentMission = null;
            isOpening = false;
    
            // 隐藏信件
            letterTransform.gameObject.SetActive(false);
            letterButton.interactable = true;

            // 处理下一个任务前检查状态
            if (mailboxNews != null)
            {
                mailboxNews.CheckMissionsStatus();
            }

            // 处理下一个任务
            ProcessNextMission();
        });
    }

    // 关闭邮箱时的处理
    private void OnCloseMailbox()
    {
        if (isProcessingMissions)
        {
            // 如果正在处理任务，先停止处理
            isProcessingMissions = false;
        
            if (isOpening)
            {
                // 如果有打开的信件，先关闭它
                HideMissionLetter();
            }
            else
            {
                // 直接关闭面板
                ClosePanelAndCheckStatus();
            }
        }
        else
        {
            // 没有任务处理时直接关闭
            ClosePanelAndCheckStatus();
        }
    }

    // 关闭面板并检查任务状态
    private void ClosePanelAndCheckStatus()
    {
        UIManager.Instance.ClosePanel(panelName);
    
        // 通知MailboxNews检查任务状态
        if (mailboxNews != null)
        {
            mailboxNews.CheckMissionsStatus();
        }
    }
    
    private void OnDestroy()
    {
        // 清理动画
        if (letterSequence != null && letterSequence.IsActive())
        {
            letterSequence.Kill();
        }
        
        // 移除事件监听
        letterButton.onClick.RemoveAllListeners();
        closePanelBtn.onClick.RemoveAllListeners();
    }
}