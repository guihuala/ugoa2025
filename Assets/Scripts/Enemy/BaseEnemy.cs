using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected StateMachine stateMachine;
    protected Animator animator;

    [Header("敌人参数设置")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float detectionRadius = 4f;
    public float detectionAngle = 60f;

    [Header("敌人的检查点配置")] public Transform CheckPoint;

    private Transform player;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>().transform;

        stateMachine = new StateMachine();
        InitializeStates();
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public bool IsPlayerDetected()
    {
        if (CheckPoint == null || player == null) return false;

        Vector3 directionToPlayer = player.position - CheckPoint.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angle = Vector3.Angle(CheckPoint.forward, directionToPlayer);

        // 进行射线检测，检测敌人与玩家之间是否有遮挡物
        RaycastHit hit;
        bool isBlocked = Physics.Raycast(CheckPoint.position, directionToPlayer.normalized, out hit, detectionRadius);

        // 只有在视野范围内 & 没有被遮挡 才能发现玩家
        if (distanceToPlayer <= detectionRadius && angle <= detectionAngle / 2)
        {
            if (isBlocked)
            {
                // 检查射线击中的是否是玩家
                if (hit.collider.CompareTag("Player"))
                {
                    return true; // 玩家没有被遮挡，敌人可以看到
                }
                else
                {
                    return false; // 视线被障碍物挡住，敌人无法发现玩家
                }
            }
            else
            {
                return true; // 直接看到玩家，没有任何遮挡
            }
        }

        return false; // 超出视野范围
    }

    
    public void ChangeState(IState newState)
    {
        stateMachine.ChangeState(newState);
    }

    protected void SetAnimationBool(string paramName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(paramName, value);
        }
    }

    protected abstract void InitializeStates();
    public abstract void MoveForward();
    public abstract void PerformFoundPlayer();
}