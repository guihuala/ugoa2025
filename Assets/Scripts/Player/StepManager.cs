using System.Collections;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    // 最大积累的格数
    public int maxSteps = 5;
    private int remainingSteps;

    // 每隔多少秒增加步数
    public float stepIncreaseInterval = 2f;

    private void Start()
    {
        remainingSteps = maxSteps;
        StartCoroutine(AutoIncreaseSteps());

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

    // 增加步数
    public void AddRemainSteps(int step)
    {
        remainingSteps = Mathf.Clamp(remainingSteps + step, 0, maxSteps);
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
}