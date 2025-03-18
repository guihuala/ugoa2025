using System.Collections;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    // 最大积累的步数
    public int maxSteps = 5;
    private int remainingSteps;

    // 每隔多少秒增加步数
    public float stepIncreaseInterval = 2f;
    private Coroutine stepIncreaseCoroutine;

    private void Start()
    {
        remainingSteps = maxSteps;
        stepIncreaseCoroutine = StartCoroutine(AutoIncreaseSteps());

        EVENTMGR.OnUseStep += UseStep;
    }

    // 消耗步数
    public void UseStep(int steps)
    {
        if (remainingSteps > 0 && remainingSteps >= steps)
        {
            remainingSteps -= steps;
            EVENTMGR.TriggerChangeSteps(remainingSteps);
        }
        else
        {
            Debug.Log("步数已用尽");
        }
    }

    // 获取当前剩余步数
    public int GetRemainingSteps()
    {
        return remainingSteps;
    }

    // 自动增加步数
    private IEnumerator AutoIncreaseSteps()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepIncreaseInterval);

            if (remainingSteps < maxSteps)
            {
                remainingSteps++;
                EVENTMGR.TriggerChangeSteps(remainingSteps);
            }
        }
    }

    // 修改步数恢复速度
    public void SetStepIncreaseInterval(float newInterval)
    {
        if (newInterval > 0)
        {
            stepIncreaseInterval = newInterval;

            // 重新启动步数恢复协程
            if (stepIncreaseCoroutine != null)
            {
                StopCoroutine(stepIncreaseCoroutine);
            }
            stepIncreaseCoroutine = StartCoroutine(AutoIncreaseSteps());
        }
    }
}
