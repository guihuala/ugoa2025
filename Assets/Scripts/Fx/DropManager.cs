using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropManager : MonoBehaviour
{
    [Header("组件配置")]
    [SerializeField] private Image imagePrefab;  // 图片预设体
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Transform Bg;
    
    [Header("图片配置")]
    [SerializeField] private Sprite[] sprites;
    
    [Header("速度配置")]
    [SerializeField] private float minSpeed = 200f;  // 最小掉落速度
    [SerializeField] private float maxSpeed = 300f;  // 最大掉落速度

    private void Update()
    {
        // 点击屏幕时生成新的掉落图片
        if (Input.GetMouseButtonDown(0)) 
        {
            CreateFallingImage();
        }
    }

    private void CreateFallingImage()
    {
        // 创建掉落的图片
        Image newImage = Instantiate(imagePrefab, Bg);
        RectTransform rectTransform = newImage.GetComponent<RectTransform>();
        
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePosition, null, out localPoint);
        
        rectTransform.anchoredPosition = localPoint + new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));
        
        newImage.sprite = sprites[Random.Range(0, sprites.Length)];
        
        float fallSpeed = Random.Range(minSpeed, maxSpeed);
        
        StartCoroutine(FallCoroutine(rectTransform, fallSpeed));
    }

    private IEnumerator FallCoroutine(RectTransform rectTransform, float speed)
    {
        while (rectTransform.anchoredPosition.y > -canvasRectTransform.rect.height)  // 超出屏幕时销毁
        {
            rectTransform.anchoredPosition += Vector2.down * speed * Time.deltaTime;
            yield return null;
        }

        // 超出屏幕后销毁图片
        Destroy(rectTransform.gameObject);
    }
}