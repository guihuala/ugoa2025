using System.Collections;
using UnityEngine;

public class MovingTrigger : MonoBehaviour, IEnterSpecialItem, IExitSpecialItem
{
    public Vector3[] movePoints;
    public float moveSpeed = 1f;

    private bool isActive = false;
    private Coroutine movementCoroutine;
    private Coroutine currentMoveCoroutine;
    
    private Transform playerTransform;
    private PlayerMovement playerMovement;
    private int currentPointIndex = 0;
    private bool isPlayerFollowing = false;
    private bool shouldStop = false;

    private void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        playerMovement = FindObjectOfType<PlayerMovement>();
        EVENTMGR.OnClickMarker += CancelFollowing;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnClickMarker -= CancelFollowing;
    }

    public void Apply()
    {
        isPlayerFollowing = true;
        shouldStop = false;

        if (!isActive && movePoints.Length > 0)
        {
            movementCoroutine = StartCoroutine(MoveAlongPath());
            isActive = true;
        }
    }

    public void Exit()
    {
        isPlayerFollowing = false;
        shouldStop = true;
    }

    private void CancelFollowing(Vector3 targetPosition)
    {
        if(targetPosition != transform.position)
        {
            isPlayerFollowing = false;
            shouldStop = true;
        }
    }

    private IEnumerator MoveAlongPath()
    {
        while (!shouldStop)
        {
            Vector3 targetPosition = movePoints[currentPointIndex];
            

            currentMoveCoroutine = StartCoroutine(MoveToPosition(targetPosition));
            yield return currentMoveCoroutine;

            if (shouldStop) break;

            currentPointIndex = (currentPointIndex + 1) % movePoints.Length;
            yield return new WaitForSeconds(.5f);
        }

        isActive = false;
        movementCoroutine = null;
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        Vector3 startPlayerPosition = playerTransform.position;
        float journeyLength = Vector3.Distance(startPosition, target);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, target) > 0.01f && !shouldStop)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            float easedT = Mathf.SmoothStep(0f, 1f, fractionOfJourney);

            transform.position = Vector3.Lerp(startPosition, target, easedT);

            if (isPlayerFollowing)
            {
                playerTransform.position = Vector3.Lerp(startPlayerPosition, 
                    target + playerMovement.PositionOffset, easedT);
            }

            yield return null;
        }

        if (!shouldStop)
        {
            transform.position = target;
            if (isPlayerFollowing)
            {
                playerTransform.position = target + playerMovement.PositionOffset;
            }
        }
    }
}