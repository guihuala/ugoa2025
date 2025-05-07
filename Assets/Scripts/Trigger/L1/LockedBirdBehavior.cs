using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LockedBirdBehavior : MonoBehaviour
{
    public Sprite[] birdSprites;

    public float jumpHeight = 1f; // 跳跃高度
    public float jumpDuration = 0.5f; // 跳跃持续时间

    private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;
    private Coroutine idleBehaviorCoroutine;
    private Vector3 originalPosition;
    private bool canMove = false;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalPosition = transform.position;
        animationCoroutine = StartCoroutine(AnimateSprite());
        
        idleBehaviorCoroutine = StartCoroutine(IdleBehavior());
    }

    public void CanMove(bool canMove)
    {
        this.canMove = canMove;
        
        if (!canMove)
        {
            idleBehaviorCoroutine = StartCoroutine(IdleBehavior());
        }
        else
        {
            if (idleBehaviorCoroutine != null)
            {
                StopCoroutine(idleBehaviorCoroutine);
                transform.position = originalPosition; // 复位位置
            }
            
            StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(FlyAnimateSprite());
        }
    }

    private IEnumerator AnimateSprite()
    {
        int index = 0;
        while (true) // 无限循环动画
        {
            spriteRenderer.sprite = birdSprites[index];
            index = (index + 1) % birdSprites.Length;
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
    
    private IEnumerator FlyAnimateSprite()
    {
        int index = 0;
        while (true) // 无限循环动画
        {
            spriteRenderer.sprite = birdSprites[index];
            index = (index + 1) % birdSprites.Length;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator IdleBehavior()
    {
        while (true)
        {
            // 随机左右翻转
            spriteRenderer.flipX = Random.value > 0.5f;
            
            // 轻微跳跃效果
            transform.DOJump(originalPosition, jumpHeight, 1, jumpDuration)
                .SetEase(Ease.OutQuad);
            
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private void OnDestroy()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        if (idleBehaviorCoroutine != null) StopCoroutine(idleBehaviorCoroutine);
    }
}