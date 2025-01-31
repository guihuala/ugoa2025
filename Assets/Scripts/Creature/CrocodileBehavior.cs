using System.Collections;
using UnityEngine;
using DG.Tweening; // Ensure you have DOTween imported

public class CrocodileBehavior : MonoBehaviour
{
    public float moveDistance = 2f;
    public float moveDuration = 1f;
    public float waitTime = 2f;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
        StartCoroutine(MoveCrocodile());
    }

    private IEnumerator MoveCrocodile()
    {
        while (true)
        {
            transform.DOMoveY(originalPosition.y + moveDistance, moveDuration).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(moveDuration + waitTime);
            
            transform.DOMoveY(originalPosition.y, moveDuration).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(moveDuration + waitTime);
        }
    }
}