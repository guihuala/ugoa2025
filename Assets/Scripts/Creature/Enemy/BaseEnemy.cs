using Spine.Unity;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected StateMachine stateMachine;
    protected SkeletonAnimation skeletonAnimation;
    private string currentAnimation = "";

    
    [Header("敌人参数设置")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float detectionRadius = 4f;
    public float detectionAngle = 60f;

    [Header("敌人的检查点配置")] public Transform CheckPoint;

    private Transform player;
    
    
    protected virtual void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
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
        
        if (distanceToPlayer <= detectionRadius && angle <= detectionAngle / 2)
        {
            if (isBlocked)
            {
                // 检查射线击中的是否是玩家
                if (hit.collider.CompareTag("Player") && !player.GetComponent<Player>().IsInvisible)
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
                if (!player.GetComponent<Player>().IsInvisible)
                    return true;

                return false;
            }
        }

        return false; // 超出视野范围
    }
    
    public void ChangeState(IState newState)
    {
        stateMachine.ChangeState(newState);
    }

    protected abstract void InitializeStates();
    public abstract void MoveForward();
    public abstract void PerformFoundPlayer();
    
    public void PlayAnimation(string animName, bool loop = true)
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            var currentTrack = skeletonAnimation.state.GetCurrent(0);
            if (currentTrack != null && currentTrack.Animation.Name == animName) 
                return;

            skeletonAnimation.state.SetAnimation(0, animName, loop);
        }
    }

    /// <summary>
    /// 表情
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="loop"></param>
    /// <param name="mixDuration"></param>
    public void PlayOverlayAnimation(int trackIndex,string animName, bool loop = false, float mixDuration = 0.1f)
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            // 设置轨道混合时间
            skeletonAnimation.state.Data.DefaultMix = mixDuration;
            skeletonAnimation.state.SetAnimation(trackIndex, animName, loop);
        }
    }

    protected virtual void ClearTrack()
    {
        skeletonAnimation.state.ClearTrack(0);
        // skeletonAnimation.state.ClearTrack(1);
        skeletonAnimation.state.ClearTrack(2);
    }
}