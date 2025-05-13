using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BirdBehavior : MonoBehaviour, IShootable
{
    public Sprite[] birdSprites;
    
    public float minFlyTime = 3f;
    public float maxFlyTime = 6f;
    public float stopTime = 2f;
    public float animationSpeed = 0.1f;
    
    public Vector3 flyAreaMin;
    public Vector3 flyAreaMax;

    [Header("被击中设置")]
    public float fallDuration = 1f; // 下落持续时间
    public float destroyDelay = 3f; // 被击中后多久销毁
    public float fallDistance = 5f; // 下落距离

    private bool isFlying = true;
    private bool isShot = false; // 是否被击中
    private float currentRotation = 0f;
    private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;
    private Coroutine actionCoroutine;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        actionCoroutine = StartCoroutine(BirdAction());
    }

    private IEnumerator BirdAction()
    {
        while (!isShot)
        {
            if (isFlying)
            {
                if (Random.value < 0.5f) // 停靠
                {
                    isFlying = false;
                    
                    if (animationCoroutine != null) StopCoroutine(animationCoroutine);
                    spriteRenderer.sprite = birdSprites[0];
                    
                    yield return new WaitForSeconds(stopTime);
                    isFlying = true;
                }
                else
                {
                    Vector3 randomTarget = new Vector3(
                        Random.Range(flyAreaMin.x, flyAreaMax.x),
                        Random.Range(flyAreaMin.y, flyAreaMax.y),
                        Random.Range(flyAreaMin.z, flyAreaMax.z)
                    );

                    float flyTime = Random.Range(minFlyTime, maxFlyTime);
                    MoveTo(randomTarget, flyTime);
                    
                    if (animationCoroutine != null) StopCoroutine(animationCoroutine);
                    animationCoroutine = StartCoroutine(AnimateSprite());

                    yield return new WaitForSeconds(flyTime);
                }
            }
        }
    }

    private void MoveTo(Vector3 target, float duration)
    {
        Vector3 direction = target - transform.position;
        if (direction != Vector3.zero)
        {
            HandleRotation(direction);
        }
    
        transform.DOMove(target, duration).SetEase(Ease.InOutSine);
    }
    
    private void HandleRotation(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            float targetRotation = direction.x > 0 ? 180f : 0f;
            if (!Mathf.Approximately(targetRotation, currentRotation))
            {
                transform.DORotate(new Vector3(0f, targetRotation, 0f), 0.3f, RotateMode.FastBeyond360);
                currentRotation = targetRotation;
            }
        }
    }

    private IEnumerator AnimateSprite()
    {
        int index = 0;
        while (isFlying && !isShot)
        {
            spriteRenderer.sprite = birdSprites[index];
            index = (index + 1) % birdSprites.Length;
            yield return new WaitForSeconds(animationSpeed);
        }
    }

    public void OnShot(BulletLifecycle bullet)
    {
        if (isShot) return; // 防止重复调用
        
        EVENTMGR.TriggerPlayerFound();
        
        isShot = true;
        
        // 停止所有行为协程
        if (actionCoroutine != null) StopCoroutine(actionCoroutine);
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        
        // 停止所有DOTween动画
        transform.DOKill();
        
        // 设置物理效果
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // 添加向下冲击力
        
        // 固定时间后销毁
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}