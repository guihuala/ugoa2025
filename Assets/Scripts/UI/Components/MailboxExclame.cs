using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MailboxNews : MonoBehaviour
{
    [Header("UI配置")]
    [SerializeField] private Transform exclamationMark;  // 感叹号标记
    [SerializeField] private float bounceHeight = 0.2f;  // 弹跳高度
    [SerializeField] private float bounceDuration = 0.5f; // 单次弹跳持续时间
    [SerializeField] private int bounceCount = 2;        // 弹跳次数
    [SerializeField] private float showScale = 1.2f;     // 显示时的缩放
    [SerializeField] private float rotationAmount = 15f; // 旋转幅度
    [SerializeField] private float shakeDuration = 0.3f; // 晃动持续时间

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Sequence bounceSequence;

    private void Awake()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        if (exclamationMark == null)
        {
            Debug.LogError("Exclamation mark reference is not set in MailboxNews!");
            return;
        }

        originalScale = exclamationMark.localScale;
        originalPosition = exclamationMark.localPosition;
        HideExclamationMark(); // 初始隐藏
    }

    private void Start()
    {
        CheckMissionsStatus();
    }

    /// <summary>
    /// 检查任务状态并更新UI
    /// </summary>
    public void CheckMissionsStatus()
    {
        if (HasPendingMissions())
        {
            ShowExclamationMark();
        }
        else
        {
            HideExclamationMark();
        }
    }

    /// <summary>
    /// 检查是否有未接受的解锁任务
    /// </summary>
    public bool HasPendingMissions()
    {
        if (LevelManager.Instance == null) return false;

        foreach (var mission in LevelManager.Instance.missions)
        {
            if (mission.isMissionUnlocked && !mission.isMissionAccepted)
            {
                return true;
            }
        }
        return false;
    }

    private void ShowExclamationMark()
    {
        if (exclamationMark == null || exclamationMark.gameObject.activeSelf) return;

        exclamationMark.gameObject.SetActive(true);
        
        // 重置状态
        exclamationMark.localScale = originalScale;
        exclamationMark.localPosition = originalPosition;
        
        // 停止之前的动画
        if (bounceSequence != null && bounceSequence.IsActive())
        {
            bounceSequence.Kill();
        }

        // 创建新的动画序列
        bounceSequence = DOTween.Sequence();
        
        // 初始放大效果
        bounceSequence.Append(exclamationMark.DOScale(originalScale * showScale, bounceDuration * 0.3f)
            .SetEase(Ease.OutBack));
        
        // 弹跳动画
        for (int i = 0; i < bounceCount; i++)
        {
            float height = i == 0 ? bounceHeight : bounceHeight * 0.5f;
            float duration = i == 0 ? bounceDuration : bounceDuration * 0.7f;
            
            bounceSequence.Append(exclamationMark.DOLocalMoveY(originalPosition.y + height, duration * 0.5f)
                .SetEase(Ease.OutQuad));
            bounceSequence.Append(exclamationMark.DOLocalMoveY(originalPosition.y, duration * 0.5f)
                .SetEase(Ease.InQuad));
        }
        
        // 添加旋转晃动效果
        bounceSequence.Append(exclamationMark.DOShakeRotation(shakeDuration, 
            new Vector3(0, 0, rotationAmount), 10, 50));
        
        // 最终恢复原始大小
        bounceSequence.Append(exclamationMark.DOScale(originalScale, bounceDuration * 0.3f));
        
        // 循环播放
        bounceSequence.SetLoops(-1, LoopType.Restart);
        bounceSequence.OnKill(() => ResetExclamationMark());
    }

    private void HideExclamationMark()
    {
        if (exclamationMark == null || !exclamationMark.gameObject.activeSelf) return;

        // 停止动画
        if (bounceSequence != null && bounceSequence.IsActive())
        {
            bounceSequence.Kill();
        }

        ResetExclamationMark();
    }

    private void ResetExclamationMark()
    {
        if (exclamationMark == null) return;

        exclamationMark.localScale = originalScale;
        exclamationMark.localPosition = originalPosition;
        exclamationMark.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        CleanupAnimation();
    }

    private void CleanupAnimation()
    {
        if (bounceSequence != null)
        {
            bounceSequence.Kill();
            bounceSequence = null;
        }
    }
}