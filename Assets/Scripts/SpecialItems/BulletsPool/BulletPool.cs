using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab; // 子弹预制体
    public int poolSize = 5;       // 对象池大小

    private Queue<GameObject> bulletQueue = new Queue<GameObject>();

    private void Awake()
    {
        // 预先实例化子弹并加入对象池
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletQueue.Enqueue(bullet);
        }
    }

    /// <summary>
    /// 获取子弹对象
    /// </summary>
    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet;
        if (bulletQueue.Count > 0)
        {
            bullet = bulletQueue.Dequeue();
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.SetActive(true);
        }
        else
        {
            // 如果池中没有可用的子弹，则实例化新的
            bullet = Instantiate(bulletPrefab, position, rotation);
        }
        return bullet;
    }

    /// <summary>
    /// 将子弹归还到对象池
    /// </summary>
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletQueue.Enqueue(bullet);
    }
}
