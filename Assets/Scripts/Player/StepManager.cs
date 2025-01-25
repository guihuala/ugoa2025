using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    public int maxSteps = 10;
    private int remainingSteps;

    private void Start()
    {
        remainingSteps = maxSteps;
    }

    // 检查是否还有步数
    public bool CanTakeStep()
    {
        return remainingSteps > 0;
    }

    // 消耗步数
    public void UseStep()
    {
        if (remainingSteps > 0)
        {
            remainingSteps--;
        }
    }

    // 重置步数
    public void ResetSteps()
    {
        remainingSteps = maxSteps;
        Debug.Log("步数已重置");
    }

    // 获取当前剩余步数
    public int GetRemainingSteps()
    {
        return remainingSteps;
    }

    //增加步数
    public void AddRemainSteps(int step)
    {
        remainingSteps += step;
    }
}
