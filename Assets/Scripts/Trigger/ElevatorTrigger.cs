using System.Collections;
using UnityEngine;

using System.Collections;
using UnityEngine;

public class ElevatorTrigger : MonoBehaviour, IEnterSpecialItem, IExitSpecialItem
{
    public Vector3 originalPosition;  // 记录物体的初始位置
    public Vector3 targetPosition;     // 目标下降位置
    public float moveSpeed = 5f;       // 物体移动的速度
    public float triggerDelay = .5f;    // 触发延迟时间（秒）

    public bool isMovingDown = false;  // 判断物体是否已经下降

    private Transform playerTransform;  // 玩家对象的位置
    private PlayerMovement playerMovement;
    private Coroutine timerCoroutine;
    private bool isPlayerOnElevator = false;

    private void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    
    public void Apply()
    {
        isPlayerOnElevator = true;
        // 启动计时器协程
        timerCoroutine = StartCoroutine(TriggerAfterDelay());
    }

    public void Exit()
    {
        isPlayerOnElevator = false;
        // 如果玩家离开时计时器还在运行，停止它
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    private IEnumerator TriggerAfterDelay()
    {
        yield return new WaitForSeconds(triggerDelay);
        
        // 检查玩家是否仍然在电梯上
        if (isPlayerOnElevator)
        {
            if (isMovingDown)
            {
                StartCoroutine(MoveToPosition(originalPosition));
            }
            else
            {
                StartCoroutine(MoveToPosition(targetPosition));
            }

            EVENTMGR.TriggerElevatorMove(isMovingDown);
            isMovingDown = !isMovingDown;
        }
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        Vector3 startPlayerPosition = playerTransform.position; // 玩家初始位置
        float journeyLength = Vector3.Distance(startPosition, target);
        float startTime = Time.time;

        // 平滑移动电梯和玩家，直到到达目标位置
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            float easedT = Mathf.SmoothStep(0f, 1f, fractionOfJourney); // 先快后慢曲线

            // 移动电梯
            transform.position = Vector3.Lerp(startPosition, target, easedT);

            // 同步移动玩家
            playerTransform.position = Vector3.Lerp(startPlayerPosition, target + playerMovement.PositionOffset, easedT);

            yield return null;
        }

        // 精确对齐最终位置
        transform.position = target;
        playerTransform.position = target + playerMovement.PositionOffset;
    }
}