using System.Collections;
using UnityEngine;
using DG.Tweening;

public class NpcShootTrigger : MonoBehaviour, IShootable
{
    [Header("精灵图设置")]
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("动画设置")]
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private int jumpCount = 1;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float fadeDuration = 0.3f;

    private bool isShowing = false;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        // 初始状态隐藏
        spriteRenderer.enabled = false;
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }

    public void OnShot(BulletLifecycle bullet)
    {
        if (isShowing) return;
        
        EVENTMGR.TriggerPlayerFound();
        ShowRandomSprite();
    }

    private void ShowRandomSprite()
    {
        isShowing = true;
        
        // 随机选择图片
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        
        // 重置状态
        spriteRenderer.enabled = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);

        // 弹跳动画
        spriteRenderer.transform.DOJump(spriteRenderer.transform.position, jumpPower, jumpCount, jumpDuration)
            .SetEase(Ease.OutQuad);
        
        // 开始隐藏协程
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        
        // 淡出动画
        spriteRenderer.DOFade(0f, fadeDuration)
            .OnComplete(() => {
                spriteRenderer.enabled = false;
                isShowing = false;
            });
    }
}