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

    [Header("点击间隔")]
    [SerializeField] private float clickCooldown = 2f; // 冷却时间 2 秒
    private bool canClick = true;  // 控制是否能点击

    private void Update()
    {
        if (canClick && Input.GetMouseButtonDown(0))
        {
            CreateFallingImage();
            StartCoroutine(ClickCooldown());  // 启动冷却时间
        }
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

    private IEnumerator ClickCooldown()
    {
        canClick = false;  // 禁止点击
        yield return new WaitForSeconds(clickCooldown);  // 等待 2 秒
        canClick = true;  // 重新允许点击
    }
}
