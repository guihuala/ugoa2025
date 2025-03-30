using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MailboxPanel : BasePanel
{
    [Header("UI组件")]
    public Transform letterTransform;   // 信件Transform
    
    [Header("动画设置")]
    public float letterPopDuration = 0.7f;
    public float letterPopHeight = 1.5f;
    public Ease popEase = Ease.OutBack;
    
    // 放在某个持久化单例里面，因为是根据关卡进度决定的，就放关卡管理器里吧
    // 只会有三个任务，根据不同主题决定
    [Header("任务配置")] 
    public MissionData[] availableMissions;
    
    private bool hasNewMail = true;
    private Vector3 letterOriginalPos;
    private Sequence letterSequence;

    private void Start()
    {
        // 保存信件原始位置
        letterOriginalPos = letterTransform.localPosition;
        
        // 初始化状态
        letterTransform.gameObject.SetActive(false);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
        AddNewMissionToSchedule();
        Interact();
    }

    // 玩家交互方法
    public void Interact()
    {
        if (hasNewMail)
        {
            DOTween.Kill(letterTransform);
            if (letterSequence != null && letterSequence.IsActive())
                letterSequence.Kill();
            
            // 播放信件弹出动画
            PlayLetterAnimation();
            
            hasNewMail = false;
        }
    }

    // 播放信件动画
    private void PlayLetterAnimation()
    {
        letterTransform.gameObject.SetActive(true);
        letterTransform.localPosition = letterOriginalPos;
        letterTransform.localScale = Vector3.zero;
        
        letterSequence = DOTween.Sequence();
        
        // 弹出动画
        letterSequence.Append(letterTransform.DOScale(1, letterPopDuration * 0.5f).SetEase(popEase));
        letterSequence.Join(letterTransform.DOLocalMoveY(letterOriginalPos.y + letterPopHeight, letterPopDuration).SetEase(Ease.OutQuad));
        
        // 轻微晃动
        letterSequence.Append(letterTransform.DOShakeRotation(0.3f, new Vector3(0, 0, 15), 10, 50));
    }
    
    
    // 添加新任务到日程板
    private void AddNewMissionToSchedule()
    {
        // 根据关卡进度添加任务
        
        
        // 用一个单例储存，通知日程板添加新任务
        
        
        // 播放音效
    }
}