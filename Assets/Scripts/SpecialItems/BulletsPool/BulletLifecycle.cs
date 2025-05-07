using UnityEngine;
using DG.Tweening;

public class BulletLifecycle : MonoBehaviour
{
    [Tooltip("子弹在场景中激活的最长时间（秒）")]
    public float lifeTime = 5f;  // 子弹存在时间
    public float fadeOutDuration = 0.5f;  // 淡出动画时长
    public GameObject hitEffectPrefab;

    private float timer = 0f;
    private bool hasReturned = false;
    private BulletPool bulletPool;
    private Transform bulletTransform;

    private void Awake()
    {
        bulletTransform = transform;
    }

    private void OnEnable()
    {
        timer = 0f;
        hasReturned = false;
        bulletTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        if (bulletPool == null)
        {
            bulletPool = FindObjectOfType<BulletPool>();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            StartFadeOut();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IShootable shootable = other.GetComponent<IShootable>();
        if (shootable != null)
        {
            HandleShootableHit(shootable);
            return;
        }
        
        EnemyBase enemyBase = other.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            HandleEnemyHit(enemyBase);
        }
    }

    // 处理实现了接口的对象
    private void HandleShootableHit(IShootable shootable)
    {
        AudioManager.Instance.PlaySfx("hit");
        shootable.OnShot(this); // 调用接口方法
        StartFadeOut();
    }

    // 处理敌人命中
    private void HandleEnemyHit(EnemyBase enemy)
    {
        AudioManager.Instance.PlaySfx("hit");
        
        EVENTMGR.TriggerPlayerFound(); // 屏幕震动
        
        SpawnHitEffect();
        StartFadeOut();
        enemy.Stun();
    }

    // 在子弹击中时生成粒子特效
    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    // 开始消失动画
    private void StartFadeOut()
    {
        if (hasReturned) return;
        hasReturned = true;
        
        bulletTransform.DOScale(Vector3.zero, fadeOutDuration)
            .SetEase(Ease.InBack)
            .OnComplete(ReturnToPool);
    }

    // 归还子弹到对象池
    private void ReturnToPool()
    {
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}