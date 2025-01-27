using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    // 状态机接口
    
    void Enter(); // 当进入此状态时调用
    void Execute(); // 状态更新逻辑，每帧调用
    void Exit(); // 当退出此状态时调用
}
