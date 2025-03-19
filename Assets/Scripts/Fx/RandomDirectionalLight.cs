using System.Collections;
using UnityEngine;

public class RandomDirectionalLight : MonoBehaviour
{
    [Header("X 轴旋转范围")]
    public float minXRotation = 30f;
    public float maxXRotation = 60f;

    [Header("Y 轴旋转范围")]
    public float minYRotation = 0f;
    public float maxYRotation = 360f;

    [Header("旋转时间 (秒)")]
    [Tooltip("旋转到新角度的时间")]
    public float rotationDuration = 3f;

    [Header("旋转间隔 (秒)")]
    [Tooltip("旋转完成后多久开始下一次旋转")]
    public float rotationInterval = 5f;

    private void Start()
    {
        StartCoroutine(PeriodicRotation());
    }

    private IEnumerator PeriodicRotation()
    {
        while (true)
        {
            // 计算新的随机角度
            float randomX = Random.Range(minXRotation, maxXRotation);
            float randomY = Random.Range(minYRotation, maxYRotation);
            Quaternion targetRotation = Quaternion.Euler(randomX, randomY, 0f);

            // 平滑旋转
            Quaternion startRotation = transform.rotation;
            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
                yield return null; // 等待下一帧
            }

            transform.rotation = targetRotation; // 确保最终角度完全匹配

            // 等待间隔时间
            yield return new WaitForSeconds(rotationInterval);
        }
    }
}