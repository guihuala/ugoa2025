using System.Collections;
using UnityEngine;

public class ResidentBehavior : MonoBehaviour
{
    public Sprite[] spriteList;
    public float animationSpeed = 0.5f;

    private SpriteRenderer spriteRenderer;
    private int index = 0;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(AnimateSprite());
    }

    private IEnumerator AnimateSprite()
    {
        while (spriteList.Length > 0)
        {
            spriteRenderer.sprite = spriteList[index];
            index = (index + 1) % spriteList.Length;
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}