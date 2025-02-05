using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterFlyUI : MonoBehaviour
{
    [SerializeField] private float minRadius = 50f; // 最小旋转半径
    [SerializeField] private float maxRadius = 150f; // 最大旋转半径
    [SerializeField] private float minSpeed = 30f;  // 最小旋转速度
    [SerializeField] private float maxSpeed = 100f;  // 最大旋转速度

    private RectTransform rectTransform;
    private Vector2 screenCenter;
    private float angle = 0f;

    // 随机的旋转半径和速度
    private float radius;
    private float speed;
    private float rotationSpeed; // 自转速度

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        // 随机初始化旋转的半径和速度
        radius = Random.Range(minRadius, maxRadius);
        speed = Random.Range(minSpeed, maxSpeed);
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