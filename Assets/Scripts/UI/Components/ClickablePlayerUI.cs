using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickablePlayerUI : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private Image image;
    private int currentIndex = 0;
    private Button triggerBtn;
    private Coroutine currentAnimation;
    private Coroutine breathingAnimation;

    private void Start()
    {
        image = GetComponent<Image>();
        if (sprites.Length > 0)
        {
            image.sprite = sprites[currentIndex];
        }

        triggerBtn = GetComponentInChildren<Button>();
        if (triggerBtn != null)
            triggerBtn.onClick.AddListener(OnMouseClick);
        
        // breathingAnimation = StartCoroutine(BreathingEffect());
    }

    public void OnMouseClick()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            transform.localScale = Vector3.one;
        }

        currentAnimation = StartCoroutine(ElasticEffect());

        currentIndex = (currentIndex + 1) % sprites.Length;
        image.sprite = sprites[currentIndex];
    }

    private IEnumerator ElasticEffect()
    {
        float duration = 0.1f;
        Vector3 originalScale = Vector3.one;
        Vector3 squishScale = new Vector3(originalScale.x * 0.75f, originalScale.y * 1.25f, originalScale.z); // 挤压效果

        float elapsed = 0;

        // 挤压
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, squishScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 还原
        elapsed = 0;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(squishScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        currentAnimation = null;
    }

    private IEnumerator BreathingEffect()
    {
        float duration = 2f; // 呼吸周期
        Vector3 originalScale = transform.localScale; // 初始大小
        Vector3 targetScale = new Vector3(1, originalScale.y * 1.05f, 1); // 呼吸最大放大倍数

        while (true) // 永久循环呼吸效果
        {
            // 放大
            float elapsed = 0;
            while (elapsed < duration)
            {
                transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 缩小
            elapsed = 0;
            while (elapsed < duration)
            {
                transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
