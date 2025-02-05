using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTexture : MonoBehaviour
{
    public float scrollSpeed = 0.5f;      // 基础横向滚动速度
    public float verticalScrollSpeed = 0.2f;  // 纵向滚动的速度
    public float verticalAmplitude = 0.05f;  // 纵向滚动的幅度

    private Material mat;
    private RectTransform rectTransform;

    private void Start()
    {
        mat = GetComponent<Image>().material;
        rectTransform = GetComponent<RectTransform>();  // 获取 RectTransform 组件
    }

    private void Update()
    {
        float dynamicScrollSpeed = scrollSpeed + Mathf.Sin(Time.time) * 0.03f;  // 滚动速度变化
        
        float offsetX = Time.time * dynamicScrollSpeed;
        float offsetY = Mathf.Sin(Time.time * verticalScrollSpeed) * verticalAmplitude;

        // 材质的横向滚动
        mat.mainTextureOffset = new Vector2(offsetX, 0);  // 只影响横向滚动，不影响纵向
        
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, offsetY);
    }
}