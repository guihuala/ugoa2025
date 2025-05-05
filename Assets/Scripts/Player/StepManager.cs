using UnityEngine;
using System.Collections;

public class StepManager : MonoBehaviour
{
    [Header("基础设置")]
    public int maxSteps = 5; // 最大步数
    private int remainingSteps; // 当前剩余步数

    [Header("动态恢复设置")]
    public float baseInterval = 1f; // 基础恢复间隔
    private float minInterval = 0.1f; // 最快恢复间隔
    private float accelerationRate = 0.7f; // 每次恢复加速比例

    private float currentInterval; // 当前实际恢复间隔
    private int consecutiveRecoveries = 0; // 连续恢复次数
    private Coroutine stepIncreaseCoroutine; // 恢复协程引用

    private void Start()
    {
        InitializeStepSystem();
    }

    public int GetRemainingSteps()
    {
        return remainingSteps;
    }

    private void InitializeStepSystem()
    {
        remainingSteps = maxSteps;
        currentInterval = baseInterval;
        stepIncreaseCoroutine = StartCoroutine(DynamicIncreaseSteps());
        
        // 注册事件
        EVENTMGR.OnUseStep += UseStep;
    }

    // 消耗步数
    public void UseStep(int steps)
    {
        if (remainingSteps > 0 && remainingSteps >= steps)
        {
            remainingSteps -= steps;
            consecutiveRecoveries = 0; // 使用步数重置连续恢复
            
            // 重置为基本速度(如果步数用尽)
            if (remainingSteps == 0)
            {
                currentInterval = baseInterval;
            }
            
            EVENTMGR.TriggerChangeSteps(remainingSteps);
        }
    }

    // 动态步数恢复协程
    private IEnumerator DynamicIncreaseSteps()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentInterval);

            if (remainingSteps < maxSteps)
            {
                remainingSteps++;
                consecutiveRecoveries++;
                
                // 计算加速后的间隔
                float newInterval = baseInterval * Mathf.Pow(accelerationRate, consecutiveRecoveries);
                currentInterval = Mathf.Max(minInterval, newInterval);

                EVENTMGR.TriggerChangeSteps(remainingSteps);
            }
            else
            {
                // 步数满时减速
                consecutiveRecoveries = 0;
                currentInterval = baseInterval;
            }
        }
    }

    // 修改基础恢复间隔(用于道具)
    public void SetStepIncreaseInterval(float multiplier)
    {
        baseInterval *= multiplier;
        baseInterval = Mathf.Clamp(baseInterval, minInterval, 10f); // 限制最小和最大间隔
    }

    private void OnDestroy()
    {
            EVENTMGR.OnUseStep -= UseStep;
        
        // 停止协程
        if (stepIncreaseCoroutine != null)
        {
            StopCoroutine(stepIncreaseCoroutine);
        }
    }
}