using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    public Sprite[] birdSprites;
    
    public float minFlyTime = 3f;
    public float maxFlyTime = 6f;
    public float stopTime = 2f;
    public float animationSpeed = 0.1f; // 控制动画播放速度

    public Transform[] perches; // 停靠点
    public Vector3 flyAreaMin;  // 随机飞行区域最小值
    public Vector3 flyAreaMax;  // 随机飞行区域最大值

    private bool isFlying = true;
    private float currentRotation = 0f;
    private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(BirdAction());
    }

    private IEnumerator BirdAction()
    {
        while (true)
        {
            if (isFlying)
            {
                if (Random.value < 0.5f && perches.Length > 0) // 停靠
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
        while (isFlying)
        {
            spriteRenderer.sprite = birdSprites[index];
            index = (index + 1) % birdSprites.Length;
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}
