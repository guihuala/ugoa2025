using System.Collections.Generic;
using UnityEngine;

public class TransparencyController : MonoBehaviour
{
    public float outerRadius = 12f; // 球壳的外半径
    public float innerRadius = 10f;  // 球壳的内半径（应小于外半径）

    private Camera playerCamera; // 玩家相机
    private Transform player; // 玩家对象
    private List<TransparencyObject> currentTransparentObjects = new List<TransparencyObject>(); // 跟踪当前所有透明物体

    private void Start()
    {
        playerCamera = FindObjectOfType<PerspectiveCameraController>().gameObject.GetComponent<Camera>();
        player = FindObjectOfType<Player>().transform;

        // 确保内半径小于外半径
        if (innerRadius >= outerRadius)
        {
            Debug.LogWarning("内半径必须小于外半径，已自动调整");
            innerRadius = outerRadius * 0.5f;
        }
    }

    void Update()
    {
        Vector3 direction = player.position - playerCamera.transform.position;

        // 获取外球体范围内的所有物体
        Collider[] outerColliders = Physics.OverlapSphere(playerCamera.transform.position, outerRadius);
        // 获取内球体范围内的所有物体
        Collider[] innerColliders = Physics.OverlapSphere(playerCamera.transform.position, innerRadius);

        // 创建一个列表来存储当前帧检测到的新透明物体
        List<TransparencyObject> newTransparentObjects = new List<TransparencyObject>();
        // 使用 HashSet 来快速排除内球体中的物体
        HashSet<Collider> innerSet = new HashSet<Collider>(innerColliders);

        // 检查外球体中的每个碰撞体
        foreach (var hit in outerColliders)
        {
            // 如果物体不在内球体范围内且不是玩家
            if (!innerSet.Contains(hit) && hit.transform != player)
            {
                TransparencyObject transparencyObject = hit.GetComponent<TransparencyObject>();
                if (transparencyObject != null)
                {
                    // 将检测到的透明物体添加到新列表中
                    newTransparentObjects.Add(transparencyObject);
                    // 如果物体尚未透明，则设置为透明
                    if (!currentTransparentObjects.Contains(transparencyObject))
                    {
                        transparencyObject.OnBecameInvisible();
                    }
                }
            }
        }

        // 恢复不再被检测到的物体的原始材质
        for (int i = currentTransparentObjects.Count - 1; i >= 0; i--)
        {
            TransparencyObject obj = currentTransparentObjects[i];
            if (!newTransparentObjects.Contains(obj))
            {
                obj.OnBecameVisible();
                currentTransparentObjects.RemoveAt(i);
            }
        }

        // 更新当前透明物体列表，添加新检测到的物体
        foreach (var newObj in newTransparentObjects)
        {
            if (!currentTransparentObjects.Contains(newObj))
            {
                currentTransparentObjects.Add(newObj);
            }
        }
    }
}