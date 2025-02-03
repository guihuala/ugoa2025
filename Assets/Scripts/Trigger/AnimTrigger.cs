using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject collectedItemPrefab;

    private GameObject player;

    private float animationDuration = .5f; // 动画时长
    private Vector3 targetScale = new Vector3(.3f, .3f, .3f); // 展示时物品的放大尺寸
    private Vector3 targetPosition = new Vector3(0, 1, 0); // 物品展示时的偏移位置

    private int totalAnimations = 0; // 记录总共要播放的动画数量
    private int completedAnimations = 0; // 记录已经完成的动画数量

    private bool callbackTriggered = false; // 确保回调只触发一次
    private List<GameObject> instantiatedItems = new List<GameObject>(); // 存储实例化的物体

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void Apply()
    {
        StartCoroutine(DisplayCollectedAchievements());
    }

    private IEnumerator DisplayCollectedAchievements()
    {
        HashSet<string> achievementList = AchievementManager.Instance.pendingAchievements;

        totalAnimations = achievementList.Count;
        completedAnimations = 0;
        callbackTriggered = false;

        // 遍历每个成就，展示已解锁的成就
        foreach (string achievement in achievementList)
        {
            GameObject instance = Instantiate(collectedItemPrefab, player.transform.position, Quaternion.identity, player.transform);

            // 设置图标
            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = AchievementManager.Instance.GetAchievementIcon(achievement);
            }
            
            instance.transform.localScale = Vector3.zero;

            // 创建动画序列：移动和放大
            Sequence seq = DOTween.Sequence();
            seq.Append(instance.transform.DOMove(player.transform.position + targetPosition, animationDuration).SetEase(Ease.OutBounce));
            seq.Join(instance.transform.DOScale(targetScale, animationDuration).SetEase(Ease.OutBack));

            seq.OnComplete(() =>
            {
                completedAnimations++;

                if (!callbackTriggered && completedAnimations == totalAnimations)
                {
                    callbackTriggered = true;
                    Debug.Log("All animations completed");
                    StartCoroutine(DestroyItemsAndTriggerCallback());
                }
            });

            // 保存实例化的物体以便后续销毁
            instantiatedItems.Add(instance);

            // 等待当前动画播放完毕再展示下一个成就
            yield return new WaitForSeconds(animationDuration + 0.5f);
        }
    }
    
    private IEnumerator DestroyItemsAndTriggerCallback()
    {
        yield return new WaitForSeconds(1f);

        // 销毁所有实例化的物体
        foreach (GameObject item in instantiatedItems)
        {
            Destroy(item);
        }
        instantiatedItems.Clear();
        
        EVENTMGR.TriggerEnterTargetField(target.transform.position);
    }
}
