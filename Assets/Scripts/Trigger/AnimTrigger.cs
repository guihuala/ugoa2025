using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject collectedItemPrefab;

    private GameObject player;
    
    private float animationDuration = 0.5f; // 动画时长
    private Vector3 targetScale = new Vector3(0.3f, 0.3f, 0.3f); // 物品放大尺寸
    private float displayRadius = 1.5f; // 物品展开的半径
    private float maxFanAngle = 120f; // 扇形的最大角度

    private int totalAnimations = 0;
    private int completedAnimations = 0;
    private bool callbackTriggered = false;

    private List<GameObject> instantiatedItems = new List<GameObject>();

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

        if (totalAnimations == 0) yield break;

        float thetaStart = -maxFanAngle / 2f; // 扇形起始角度
        float thetaStep = (totalAnimations > 1) ? maxFanAngle / (totalAnimations - 1) : 0; // 角度间隔

        int index = 0;
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

            // 计算扇形排列位置
            float angle = thetaStart + index * thetaStep;
            float radians = angle * Mathf.Deg2Rad;
            Vector3 targetPos = player.transform.position + new Vector3(Mathf.Cos(radians) * displayRadius, 1f, Mathf.Sin(radians) * displayRadius);
            
            Sequence seq = DOTween.Sequence();
            seq.Append(instance.transform.DOMove(targetPos, animationDuration).SetEase(Ease.OutBounce));
            seq.Join(instance.transform.DOScale(targetScale, animationDuration).SetEase(Ease.OutBack));
            seq.Join(instance.transform.DORotate(new Vector3(1,0,1) * angle, animationDuration)); // 旋转使其朝向中心

            seq.OnComplete(() =>
            {
                completedAnimations++;

                if (!callbackTriggered && completedAnimations == totalAnimations)
                {
                    callbackTriggered = true;
                    StartCoroutine(DestroyItemsAndTriggerCallback());
                }
            });

            instantiatedItems.Add(instance);

            index++;
            yield return new WaitForSeconds(0.2f); // 依次播放动画，避免所有物品同时展开
        }
    }

    private IEnumerator DestroyItemsAndTriggerCallback()
    {
        yield return new WaitForSeconds(1f);

        foreach (GameObject item in instantiatedItems)
        {
            Destroy(item);
        }
        instantiatedItems.Clear();

        EVENTMGR.TriggerEnterTargetField(target.transform.position);
    }
}
