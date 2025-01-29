using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected StateMachine stateMachine;
    protected Animator animator;

    [Header("敌人参数设置")] public Transform[] patrolPoints;
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
        float angle = Vector3.Angle(CheckPoint.forward, directionToPlayer);
    
        // 计算射线是否被遮挡
        RaycastHit hit;
        bool isBlocked = Physics.Raycast(CheckPoint.position, directionToPlayer.normalized, out hit, detectionRadius, LayerMask.GetMask("Tree"));

        // 检测玩家是否在检测角度和范围内，同时不能被遮挡
        return directionToPlayer.magnitude <= detectionRadius && angle <= detectionAngle / 2 && !isBlocked;
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

    // 动态可视化侦查区域
    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || CheckPoint == null) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);

        // 从检查点出发
        Vector3 forward = CheckPoint.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * forward;

        // 画出侦查扇形范围
        Gizmos.DrawWireSphere(CheckPoint.position, detectionRadius);
        Gizmos.DrawLine(CheckPoint.position, CheckPoint.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(CheckPoint.position, CheckPoint.position + rightBoundary * detectionRadius);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.1f);
        for (int i = 0; i <= 30; i++)
        {
            float angleStep = -detectionAngle / 2 + (detectionAngle / 30) * i;
            Vector3 startDir = Quaternion.Euler(0, angleStep, 0) * forward;
            Vector3 endDir = Quaternion.Euler(0, angleStep + detectionAngle / 30, 0) * forward;

            Gizmos.DrawLine(CheckPoint.position + startDir * detectionRadius,
                CheckPoint.position + endDir * detectionRadius);
        }
    }

    protected abstract void InitializeStates();
    public abstract void MoveForward();
    public abstract void PerformFoundPlayer();
}