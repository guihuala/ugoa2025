using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ButtonTrigger : MonoBehaviour, IShootable
{
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private float terrainUpdateDelay = 0.5f; // 延迟通知时间

    private bool hasTriggered = false;

    public void OnShot(BulletLifecycle bullet)
    {
        if (hasTriggered) return;

        if (timelineDirector != null)
        {
            PerspectiveCameraController perspectiveCameraController = Camera.main.GetComponent<PerspectiveCameraController>();
            if (perspectiveCameraController != null)
            {
                perspectiveCameraController.ShakeCamera(1);
            }

            // 播放Timeline
            timelineDirector.Play();
            
            // 延迟通知寻路管理器
            StartCoroutine(DelayedTerrainUpdate());
        }

        hasTriggered = true;
    }

    private IEnumerator DelayedTerrainUpdate()
    {
        // 等待Timeline开始播放
        yield return new WaitForSeconds(terrainUpdateDelay);
        
        // 通知寻路管理器重新搜索地图
        EVENTMGR.TriggerTerrainChange();
    }
}