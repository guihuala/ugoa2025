using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : Singleton<TutorialManager>
{
    [Header("教程步骤配置")]
    public RectTransform mailboxHighlightTarget; // 邮箱UI目标
    public RectTransform scheduleButtonTarget;  // 日程板按钮目标
    public RectTransform missionButtonTarget;   // 任务按钮目标
    
    [Header("UI组件")]
    public GuideMask guideMask;
    public Text tutorialText;
    public Image handPointer;
    
    [Header("动画设置")]
    public float textFadeDuration = 0.5f;
    public float pointerMoveDuration = 1f;
    
    private int currentStep = 0;
    private Sequence pointerSequence;

    private void Start()
    {
        // 初始化遮罩
        guideMask.Init();
        
        // 开始教程
        StartTutorial();
    }

    public void StartTutorial()
    {
        // 检查是否已经完成过教程
        if (PlayerPrefs.HasKey("Tutorial_Completed"))
        {
            return;
        }
        
        currentStep = 0;
        NextStep();
    }

    private void NextStep()
    {
        currentStep++;
        
        // 停止之前的动画
        if (pointerSequence != null && pointerSequence.IsActive())
        {
            pointerSequence.Kill();
        }
        
        // 根据步骤执行不同操作
        switch (currentStep)
        {
            case 1:
                ShowMailboxStep();
                break;
            case 2:
                ShowScheduleStep();
                break;
            case 3:
                ShowMissionStep();
                break;
            case 4:
                CompleteTutorial();
                break;
        }
    }

    #region 具体步骤

    private void ShowMailboxStep()
    {
        // 高亮邮箱
        guideMask.Play(mailboxHighlightTarget);
        
        // 更新提示文本
        tutorialText.DOFade(0, textFadeDuration).OnComplete(() =>
        {
            tutorialText.text = "点击邮箱查看新邮件";
            tutorialText.DOFade(1, textFadeDuration);
        });
        
        // 显示手指指示动画
        handPointer.gameObject.SetActive(true);
        handPointer.rectTransform.position = mailboxHighlightTarget.position + Vector3.up * 100f;
        
        pointerSequence = DOTween.Sequence();
        pointerSequence.Append(handPointer.rectTransform.DOMoveY(mailboxHighlightTarget.position.y - 50f, pointerMoveDuration).SetEase(Ease.InOutSine));
        pointerSequence.Append(handPointer.rectTransform.DOMoveY(mailboxHighlightTarget.position.y + 50f, pointerMoveDuration).SetEase(Ease.InOutSine));
        pointerSequence.SetLoops(-1);
    }

    private void ShowScheduleStep()
    {
        // 高亮日程板按钮
        guideMask.Play(scheduleButtonTarget);
        
        // 更新提示文本
        tutorialText.DOFade(0, textFadeDuration).OnComplete(() =>
        {
            tutorialText.text = "打开日程板查看新任务";
            tutorialText.DOFade(1, textFadeDuration);
        });
        
        // 更新手指指示位置
        handPointer.rectTransform.position = scheduleButtonTarget.position + Vector3.up * 100f;
        
        pointerSequence = DOTween.Sequence();
        pointerSequence.Append(handPointer.rectTransform.DOMoveY(scheduleButtonTarget.position.y - 50f, pointerMoveDuration).SetEase(Ease.InOutSine));
        pointerSequence.Append(handPointer.rectTransform.DOMoveY(scheduleButtonTarget.position.y + 50f, pointerMoveDuration).SetEase(Ease.InOutSine));
        pointerSequence.SetLoops(-1);
    }

    private void ShowMissionStep()
    {
        // 高亮任务按钮
        guideMask.Play(missionButtonTarget);
        
        // 更新提示文本
        tutorialText.DOFade(0, textFadeDuration).OnComplete(() =>
        {
            tutorialText.text = "点击任务查看详细信息";
            tutorialText.DOFade(1, textFadeDuration);
        });
        
        // 更新手指指示位置
        handPointer.rectTransform.position = missionButtonTarget.position + Vector3.up * 100f;
        
        pointerSequence = DOTween.Sequence();
        pointerSequence.Append(handPointer.rectTransform.DOMoveY(missionButtonTarget.position.y - 50f, pointerMoveDuration).SetEase(Ease.InOutSine));
        pointerSequence.Append(handPointer.rectTransform.DOMoveY(missionButtonTarget.position.y + 50f, pointerMoveDuration).SetEase(Ease.InOutSine));
        pointerSequence.SetLoops(-1);
    }    

    #endregion
    
    private void CompleteTutorial()
    {
        // 标记教程完成
        PlayerPrefs.SetInt("Tutorial_Completed", 1);
        
        // 隐藏所有教程元素
        guideMask.Close();
        tutorialText.DOFade(0, textFadeDuration);
        handPointer.gameObject.SetActive(false);
        
        // 显示完成提示
    }

    // 当玩家完成当前步骤时
    public void OnStepCompleted()
    {
        NextStep();
    }
    
    public void SkipTutorial()
    {
        PlayerPrefs.SetInt("Tutorial_Completed", 1);
        guideMask.Close();
        tutorialText.gameObject.SetActive(false);
        handPointer.gameObject.SetActive(false);
    }
}