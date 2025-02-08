using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // 添加 EventSystems 命名空间

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
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            CreateFallingImage();
        }
    }

    private bool IsPointerOverUIObject()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void CreateFallingImage()
    {
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
