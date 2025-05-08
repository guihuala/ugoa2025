using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FishJumpBehavior : MonoBehaviour
{
    [Header("跳跃参数")]
    public float jumpHeight = 1.5f;      // 跳跃高度
    public float jumpDistance = 2f;      // 跳跃水平距离
    public float jumpDuration = 1f;      // 跳跃持续时间
    public float minWaitTime = 3f;       // 最短等待时间
    public float maxWaitTime = 8f;       // 最长等待时间

    private Vector3 originalPosition;    // 初始位置
    private bool isJumping = false;      // 是否正在跳跃

    private void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(RandomJumpRoutine());
    }

    private IEnumerator RandomJumpRoutine()
    {
        while (true)
        {
            // 随机等待时间
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 如果已经在跳跃则跳过
            if (isJumping) continue;
            
            StartCoroutine(PerformJump());
        }
    }

    private IEnumerator PerformJump()
    {
        isJumping = true;
        
        // 随机决定跳跃方向（左或右）
        float direction = Random.value > 0.5f ? 1f : -1f;
        Vector3 jumpTarget = originalPosition + 
                             new Vector3(jumpDistance * direction, 0, 0);

        // 抛物线跳跃
        transform.DOJump(jumpTarget, jumpHeight, 1, jumpDuration)
            .SetEase(Ease.OutQuad);
        
        yield return new WaitForSeconds(jumpDuration);
        
        // 返回水中（直接重置位置）
        transform.position = originalPosition;
        isJumping = false;
    }

    // 防止编辑器模式下位置被修改
    private void OnValidate()
    {
        if (Application.isPlaying && originalPosition != Vector3.zero)
        {
            transform.position = originalPosition;
        }
    }
}