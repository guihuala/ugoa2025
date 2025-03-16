using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameBaseUI : MonoBehaviour
{
    [Header("组件配置")]
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Transform stepContainer;
    [SerializeField] private Image[] stepImages;
    
    [Header("动画配置")]
    [SerializeField] private Vector3 cardScaleIn = new Vector3(0f, 0f, 1f);  // 消失时的缩放（缩到 0）
    [SerializeField] private Vector3 cardScaleNormal = new Vector3(1f, 1f, 1f); // 正常大小

    private void Awake()
    {
        pauseBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenPanel("SettingPanel");
        });
    }

    private void Start()
    {
        if(stepContainer != null)
            EVENTMGR.ChangeSteps += UpdateTransform;
    }

    private void OnDestroy()
    {
        EVENTMGR.ChangeSteps -= UpdateTransform;
    }

    /// <summary>
    /// 根据传入的剩余步数更新 UI 图片显示：
    /// 步数减少时，从数组头部（索引低处）依次隐藏激活的图片
    /// 步数恢复时，从数组尾部依次显示（播放恢复动画）隐藏的图片
    /// </summary>
    /// <param name="remainedStep">剩余步数</param>
    private void UpdateTransform(int remainedStep)
    {
        // 统计当前激活的图片数量
        int activeCount = 0;
        foreach (var img in stepImages)
        {
            if (img.gameObject.activeSelf)
                activeCount++;
        }

        if (activeCount > remainedStep)
        {
            // 步数减少：需要隐藏 activeCount - remainedStep 个图片
            int countToRemove = activeCount - remainedStep;
            // 遍历数组，从前往后，隐藏第一个激活的图片
            for (int i = 0; i < stepImages.Length && countToRemove > 0; i++)
            {
                if (stepImages[i].gameObject.activeSelf)
                {
                    StartCoroutine(AnimateImageDisappear(stepImages[i].gameObject));
                    countToRemove--;
                }
            }
        }
        else if (activeCount < remainedStep)
        {
            // 步数恢复：需要显示 remainedStep - activeCount 个图片
            int countToAdd = remainedStep - activeCount;
            // 从后往前，恢复最后一个隐藏的图片
            for (int i = stepImages.Length - 1; i >= 0 && countToAdd > 0; i--)
            {
                if (!stepImages[i].gameObject.activeSelf)
                {
                    // 先设置为激活状态，再播放出现动画
                    stepImages[i].gameObject.SetActive(true);
                    StartCoroutine(AnimateImage(stepImages[i].gameObject));
                    countToAdd--;
                }
            }
        }
    }
    
    /// <summary>
    /// 图片出现动画：从缩小状态放大到正常大小
    /// </summary>
    private IEnumerator AnimateImage(GameObject card)
    {
        RectTransform rectTransform = card.GetComponent<RectTransform>();
        // 初始设为缩小状态
        rectTransform.localScale = cardScaleIn;
        
        float elapsed = 0f;
        float duration = 0.2f;
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(cardScaleIn, cardScaleNormal, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        rectTransform.localScale = cardScaleNormal;
    }
    
    /// <summary>
    /// 图片消失动画：从正常大小缩放到消失状态，然后隐藏 GameObject
    /// </summary>
    private IEnumerator AnimateImageDisappear(GameObject card)
    {
        RectTransform rectTransform = card.GetComponent<RectTransform>();
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 startScale = cardScaleNormal;
        Vector3 targetScale = cardScaleIn;
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = targetScale;
        // 动画结束后设置为不激活状态
        card.SetActive(false);
    }
}
