using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TargetTrigger : MonoBehaviour,IEnterSpecialItem
{
    // 触发剧情里的上下台阶的剧情过渡用的
    // 不会对目标方块路径上的方块是否可行走进行检查
    // 使用的时候可以将路径上的方块是否可行走设置为否
    
    // 其他的复杂一些的剧情可以直接放个CG
    
    [SerializeField] private GameObject target;

    public void Apply()
    {
        EVENTMGR.TriggerEnterTargetField(target.transform.position);
    }
}