using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButterFlyUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 screenCenter;
    private float angle = 0f;

    // 旋转参数
    private float radius;
    private float baseSpeed;
    private float speed;
    private float rotationSpeed; // 自转速度

    private float speedVariation; // 速度变化范围
    private float speedFrequency; // 速度变化频率

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        // 根据屏幕大小动态调整旋转半径
        float screenMinSize = Mathf.Min(Screen.width, Screen.height);
        float minRadius = screenMinSize * 0.7f;
        float maxRadius = screenMinSize * 0.9f;

        // 随机初始化旋转半径和速度
        radius = Random.Range(minRadius, maxRadius);
        baseSpeed = Random.Range(30f, 100f);  // 基础速度
        speedVariation = baseSpeed * 0.5f;  // 速度变化范围（最大速度波动 50%）
        speedFrequency = Random.Range(0.5f, 1.5f);  // 速度变化频率

        rotationSpeed = baseSpeed;
    }

    private void Update()
    {
        // 让速度随时间变化，模拟时快时慢的飞行
        speed = baseSpeed + Mathf.Sin(Time.time * speedFrequency) * speedVariation;

        angle += speed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        float radian = angle * Mathf.Deg2Rad;
        float x = screenCenter.x + radius * Mathf.Cos(radian);
        float y = screenCenter.y + radius * Mathf.Sin(radian);

        rectTransform.position = new Vector3(x, y, rectTransform.position.z);

        // 实现自转（速度变化影响自转）
        rectTransform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}
