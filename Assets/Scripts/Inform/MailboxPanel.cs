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

    // 任务相关变量
    private Queue<MissionData> pendingMissions = new Queue<MissionData>();
    private MissionData currentMission;
    private bool isOpening = false;

    protected override void Awake()
    {
        base.Awake();
        // 保存信件原始位置
        letterOriginalPos = letterTransform.localPosition;
    }

    private void Start()
    {
        letterButton.onClick.AddListener(HideMissionLetter);
        closePanelBtn.onClick.AddListener(OnCloseMailbox);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);

        // 初始化任务队列
        InitPendingMissions();

        // 开始处理任务
        ProcessNextMission();
    }

    // 初始化待处理任务队列
    private void InitPendingMissions()
    {
        pendingMissions.Clear();

        // 获取所有已解锁但未接受的任务
        foreach (var mission in LevelManager.Instance.missions)
        {
            if (mission.isMissionUnlocked && !mission.isMissionAccepted)
            {
                pendingMissions.Enqueue(mission);
            }
        }
    }

    // 处理下一个任务
    private void ProcessNextMission()
    {
        // 如果没有打开的信件且有任务待处理
        if (!isOpening && pendingMissions.Count > 0)
        {
            currentMission = pendingMissions.Dequeue();
            ShowMissionLetter(currentMission);
        }
        else if (!isOpening && pendingMissions.Count == 0)
        {
            // 没有更多任务时直接关闭邮箱
            UIManager.Instance.ClosePanel(panelName);
        }
    }

    // 显示任务信件
    private void ShowMissionLetter(MissionData mission)
    {
        isOpening = true;

        missionTitle.text = mission.missionTitle;
        missionDescriptionText.text = mission.missionDescription;

        // 重置信件位置和大小
        letterTransform.localPosition = letterOriginalPos;
        letterTransform.localScale = Vector3.zero;
        letterTransform.gameObject.SetActive(true);
        
        // 停止之前的动画
        if (letterSequence != null && letterSequence.IsActive())
            letterSequence.Kill();

        // 播放信件弹出动画
        OpenLetterAnimation();
    }

    private void HideMissionLetter()
    {
        if (currentMission != null && isOpening)
        {
            CloseLetterAnimation();
        }
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
        });
    }

    // 播放信件关闭动画
    private void CloseLetterAnimation()
    {
        // 停止之前的动画
        DOTween.Kill(letterTransform);
        if (letterSequence != null && letterSequence.IsActive())
            letterSequence.Kill();

        letterSequence = DOTween.Sequence();

        // 关闭动画 - 下落并缩小
        letterSequence.Append(letterTransform.DOLocalMoveY(letterOriginalPos.y, letterCloseDuration).SetEase(closeEase));
        letterSequence.Join(letterTransform.DOScale(0, letterCloseDuration).SetEase(closeEase));

        letterSequence.OnComplete(() =>
        {
            // 接受当前显示的任务
            LevelManager.Instance.AcceptMission(currentMission.missionID);
            currentMission = null;

            // 隐藏信件
            letterTransform.gameObject.SetActive(false);
            isOpening = false;

            // 更新MailboxNews的状态
            MailboxNews news = FindObjectOfType<MailboxNews>();
            if (news != null)
            {
                news.HasPendingMissions();
            }

            // 处理下一个任务
            ProcessNextMission();
        });
    }

    // 关闭邮箱时的处理
    private void OnCloseMailbox()
    {
        if (currentMission == null && !isOpening)
        {
            // 没有任务时直接关闭
            UIManager.Instance.ClosePanel(panelName);
        }
        else if (isOpening)
        {
            // 有任务时先关闭当前信件
            HideMissionLetter();
        }
    }
}