using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private int expandSize = 5;
    
    private Queue<GameObject> bulletQueue = new Queue<GameObject>();

    private void Awake()
    {
        ExpandPool(initialPoolSize);
    }

    private void ExpandPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            bulletQueue.Enqueue(bullet);
        }
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        if (bulletQueue.Count == 0)
        {
            ExpandPool(expandSize);
            Debug.LogWarning("Bullet pool expanded!");
        }

        GameObject bullet = bulletQueue.Dequeue();
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bullet.transform.SetParent(transform);
        bulletQueue.Enqueue(bullet);
    }
}
