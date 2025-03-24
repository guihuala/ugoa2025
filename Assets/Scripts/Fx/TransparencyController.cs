using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyController : MonoBehaviour
{
    public float raycastDistance = 10f; // 射线检测范围

    private Camera playerCamera; // 玩家相机
    private Transform player; // 玩家对象
    private TransparencyObject lastHitTransparencyObject = null; // 记录上一个命中的透明物体

    private void Start()
    {
        playerCamera = FindObjectOfType<PerspectiveCameraController>().gameObject.GetComponent<Camera>();
        player = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        Vector3 direction = player.position - playerCamera.transform.position;

        // 获取相机和玩家之间区域内的所有物体
        Collider[] hitColliders = Physics.OverlapSphere(playerCamera.transform.position, raycastDistance);

        bool isHitAnyObject = false;

        foreach (var hit in hitColliders)
        {
            if (hit.transform != player) // 如果物体不是玩家
            {
                // 检查物体是否包含 TransparencyObject 脚本
                TransparencyObject hitTransparencyObject = hit.GetComponent<TransparencyObject>();
                if (hitTransparencyObject != null)
                {
                    // 如果之前有物体命中且与当前命中的物体不同，恢复之前的物体材质
                    if (lastHitTransparencyObject != null && lastHitTransparencyObject != hitTransparencyObject)
                    {
                        lastHitTransparencyObject.OnBecameVisible(); // 恢复原材质
                    }

                    // 设置当前命中的物体为透明
                    hitTransparencyObject.OnBecameInvisible();
                    lastHitTransparencyObject = hitTransparencyObject; // 更新为当前命中的物体

                    isHitAnyObject = true; // 标记已经命中物体
                }
            }
        }

        // 如果没有命中物体，恢复之前命中的物体材质
        if (!isHitAnyObject && lastHitTransparencyObject != null)
        {
            lastHitTransparencyObject.OnBecameVisible(); // 恢复原材质
            lastHitTransparencyObject = null; // 清除上一个透明物体的记录
        }
    }
}
