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
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return directionToPlayer.magnitude <= detectionRadius && angle <= detectionAngle / 2;
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
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // 设置侦查区域颜色（橙色半透明）

        // 绘制侦查范围的扇形
        Vector3 forward = transform.forward; // 当前移动方向
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * forward;

        // 绘制扇形区域
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // 外圆形边界
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);

        // 填充扇形区域
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.1f); // 更透明的橙色
        for (int i = 0; i <= 30; i++) // 每 1°绘制一小段线
        {
            float angleStep = -detectionAngle / 2 + (detectionAngle / 30) * i;
            Vector3 startDir = Quaternion.Euler(0, angleStep, 0) * forward;
            Vector3 endDir = Quaternion.Euler(0, angleStep + detectionAngle / 30, 0) * forward;

            Gizmos.DrawLine(transform.position + startDir * detectionRadius, transform.position + endDir * detectionRadius);
        }
    }

    protected abstract void InitializeStates();
    public abstract void MoveForward();
    public abstract void PerformFoundPlayer();
}
