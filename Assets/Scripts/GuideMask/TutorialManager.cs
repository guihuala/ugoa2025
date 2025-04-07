using System.Collections;
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
    public RectTransform tutorialTextPanel; // 文字提示面板
    public Text tutorialText;
    public Image handPointer;
    
    [Header("动画设置")]
    public float textFadeDuration = 0.5f;
    public float pointerClickAnimationDuration = 0.3f;
    public float stepTransitionDelay = 0.5f; // 步骤间延迟时间
    
    [Header("教程流程控制")]
    public int startStep = 1; // 默认从第一步开始
    public int endStep = 4;   // 默认到第四步结束
    
    private int currentStep = 0;
    private Sequence pointerSequence;
    private bool isWaitingForInput = false;
    private bool isTransitioning = false; // 是否正在过渡到下一步

    private void Start()
    {
        // 初始化遮罩
        guideMask.Init();
        guideMask.OnClickOutside += OnClickOutsideMask;
        
        // 开始教程
        StartTutorial();
    }

    private void OnDestroy()
    {
        if (guideMask != null)
        {
            guideMask.OnClickOutside -= OnClickOutsideMask;
        }
    }

    public void StartTutorial()
    {
        // 检查是否已经完成过教程
        if (PlayerPrefs.HasKey("Tutorial_Completed"))
        {
            CompleteTutorial();
            return;
        }
        
        currentStep = Mathf.Clamp(startStep, 1, 4); // 确保开始步骤在有效范围内
        NextStep();
    }

    private void NextStep()
    {
        isWaitingForInput = false;
        
        // 停止之前的动画
        if (pointerSequence != null && pointerSequence.IsActive())
        {
            pointerSequence.Kill();
        }
        
        Debug.Log($"进入教程步骤: {currentStep}");
        
        // 检查是否到达终点步骤
        if (currentStep > endStep)
        {
            CompleteTutorial();
            return;
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
            default:
                // 不应该到达这里
                Debug.LogError($"未知的教程步骤: {currentStep}");
                CompleteTutorial();
                break;
        }
    }

    #region 具体步骤

    private void ShowMailboxStep()
    {
        // 高亮邮箱
        guideMask.Play(mailboxHighlightTarget);
        
        // 更新提示文本位置和内容
        UpdateTextPosition(mailboxHighlightTarget.position);
        UpdateTextContent("回工作室睡个觉吧");
        
        // 显示手指指示动画
        PlayPointerAnimation(mailboxHighlightTarget.position);
        
        isWaitingForInput = true;
    }

    private void ShowScheduleStep()
    {
        // 高亮日程板按钮
        guideMask.Play(scheduleButtonTarget);
        
        // 更新提示文本位置和内容
        UpdateTextPosition(scheduleButtonTarget.position);
        UpdateTextContent("回工作室睡个觉吧");
        
        // 更新手指指示动画
        PlayPointerAnimation(scheduleButtonTarget.position);
        
        isWaitingForInput = true;
    }

    private void ShowMissionStep()
    {
        // 高亮任务按钮
        guideMask.Play(missionButtonTarget);
        
        // 更新提示文本位置和内容
        UpdateTextPosition(missionButtonTarget.position);
        UpdateTextContent("怎么有新邮件？看看吧");
        
        // 更新手指指示动画
        PlayPointerAnimation(missionButtonTarget.position);
        
        isWaitingForInput = true;
    }    

    #endregion
    
    private void UpdateTextContent(string content)
    {
        tutorialText.DOFade(0, textFadeDuration).OnComplete(() =>
        {
            tutorialText.text = content;
            tutorialText.DOFade(1, textFadeDuration);
        });
    }
    
    private void UpdateTextPosition(Vector3 targetPosition)
    {
        Vector3 textPosition = new Vector3(
            targetPosition.x + 100f,
            targetPosition.y + 200f,
            targetPosition.z
        );
        
        tutorialTextPanel.DOMove(textPosition, textFadeDuration).SetEase(Ease.OutQuad);
    }
    
    private void PlayPointerAnimation(Vector3 targetPosition)
    {
        handPointer.gameObject.SetActive(true);
        handPointer.rectTransform.position = targetPosition;
        
        // 创建点击动画
        pointerSequence = DOTween.Sequence();
        pointerSequence.Append(handPointer.rectTransform.DOScale(1.2f, pointerClickAnimationDuration/2).SetEase(Ease.OutQuad));
        pointerSequence.Append(handPointer.rectTransform.DOScale(1f, pointerClickAnimationDuration/2).SetEase(Ease.InQuad));
        pointerSequence.SetLoops(-1);
    }
    
    private void OnClickOutsideMask()
    {
        if (isWaitingForInput && !isTransitioning)
        {
            CompleteCurrentStep();
        }
    }
    
    private void CompleteCurrentStep()
    {
        isWaitingForInput = false;
        isTransitioning = true;
        
        // 添加延迟后再进入下一步
        StartCoroutine(DelayedNextStep());
    }
    
    private IEnumerator DelayedNextStep()
    {
        yield return new WaitForSeconds(stepTransitionDelay);
        
        currentStep++;
        isTransitioning = false;
        NextStep();
    }

    private void CompleteTutorial()
    {
        // 只有当完成完整教程时才保存完成状态
        if (endStep == 4)
        {
            PlayerPrefs.SetInt("Tutorial_Completed", 1);
        }
        
        // 清理UI元素
        if (guideMask != null) guideMask.Close();
        if (tutorialText != null) tutorialText.DOFade(0, textFadeDuration);
        if (handPointer != null) handPointer.gameObject.SetActive(false);
        
        // 延迟销毁确保动画完成
        StartCoroutine(DelayedSelfDestruct());
    }

    private IEnumerator DelayedSelfDestruct()
    {
        yield return new WaitForSeconds(1f); // 等待所有动画完成
        
        // 销毁前确保清理所有引用
        if (pointerSequence != null) pointerSequence.Kill();
        if (guideMask != null) Destroy(guideMask.gameObject);
        
        Destroy(gameObject);
    }
}