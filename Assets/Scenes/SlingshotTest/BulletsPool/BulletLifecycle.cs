using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLifecycle : MonoBehaviour
{
    [Tooltip("子弹在场景中激活的最长时间（秒）")]
    public float lifeTime = 5f;  // 子弹存在时间

    private float timer = 0f;
    private bool hasReturned = false;
    private BulletPool bulletPool;

    // 当子弹被启用时，重置计时器和状态
    private void OnEnable()
    {
        timer = 0f;
        hasReturned = false;
        // 如果对象池引用为空，则查找场景中的 BulletPool 实例
        if (bulletPool == null)
        {
            bulletPool = FindObjectOfType<BulletPool>();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        // 超过设定时间后归还子弹
        if (timer > lifeTime)
        {
            ReturnToPool();
        }
    }

    // 当子弹发生碰撞时归还子弹到对象池
    private void OnCollisionEnter(Collision collision)
    {
        ReturnToPool();
    }

    // 将子弹归还到对象池或直接禁用
    private void ReturnToPool()
    {
        if (hasReturned) return;
        hasReturned = true;
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(gameObject);
        }
        else
        {
            // 如果找不到对象池，则直接禁用子弹
            gameObject.SetActive(false);
        }
    }
}
