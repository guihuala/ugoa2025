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
    private float speed;
    private float rotationSpeed; // 自转速度

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        // 根据屏幕大小动态调整旋转半径
        float screenMinSize = Mathf.Min(Screen.width, Screen.height);
        float minRadius = screenMinSize * 0.5f;  // 旋转半径下限 = 屏幕最小边长的 10%
        float maxRadius = screenMinSize * 0.6f;  // 旋转半径上限 = 屏幕最小边长的 30%

        // 随机初始化旋转半径和速度
        radius = Random.Range(minRadius, maxRadius);
        speed = Random.Range(30f, 100f);  // 保持固定速度范围
        rotationSpeed = speed;
    }

    private void Update()
    {
        angle += speed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        float radian = angle * Mathf.Deg2Rad;
        float x = screenCenter.x + radius * Mathf.Cos(radian);
        float y = screenCenter.y + radius * Mathf.Sin(radian);

        rectTransform.position = new Vector3(x, y, rectTransform.position.z);

        // 实现自转
        rectTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}