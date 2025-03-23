using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image itemIcon;
    public Image cooldownIcon;
    public Image effectDurationIcon;
    
    private ItemData item;
    private Button itemButton;
    private PlayerItemEffect player;
    
    private float currentTime; // 当前冷却时间
    private float cooldown;    // 冷却时长
    private float effectTime;  // 当前生效时间
    private bool isEffectActive; // 判断物品是否生效中

    private void Start()
    {
        player = FindObjectOfType<PlayerItemEffect>();
        itemButton = GetComponent<Button>(); // 获取Button组件
        itemIcon = transform.GetChild(0).GetComponent<Image>();

        if (item != null)
        {
            itemIcon.sprite = item.icon;
            cooldown = item.cooldownTime; // 设定冷却时间
            effectTime = item.effectDuration; // 设定生效时长
        }

        itemButton.onClick.AddListener(() => UseItem());
    }

    private void Update()
    {
        UpdateEffectDuration();  // 始终更新生效时间
        UpdateCooldowns();  // 只有在生效时间结束后才更新冷却时间
        UpdateButtonState();  // 更新按钮状态
    }

    // 更新冷却时间
    private void UpdateCooldowns()
    {
        if (!isEffectActive && currentTime > 0) // 确保只有在生效时间结束后才更新冷却时间
        {
            currentTime -= Time.deltaTime;
            cooldownIcon.fillAmount = currentTime / cooldown;
        }
    }

    // 更新物品生效时长
    private void UpdateEffectDuration()
    {
        if (isEffectActive && effectTime > 0)
        {
            effectTime -= Time.deltaTime;
            effectDurationIcon.fillAmount = effectTime / item.effectDuration;
        }

        if (effectTime <= 0 && isEffectActive)
        {
            isEffectActive = false; // 停止生效
            effectDurationIcon.fillAmount = 0; // 重置生效时间显示
            currentTime = cooldown; // 生效时间结束后开始冷却
        }
    }

    // 更新按钮状态（冷却时禁用按钮）
    private void UpdateButtonState()
    {
        if (item != null)
        {
            itemButton.interactable = (currentTime <= 0 && !isEffectActive); // 只有在冷却时间结束且物品未生效时按钮才可用
        }
    }

    // 更新道具信息
    public void UpdateItemInfo(ItemData newItem)
    {
        this.item = newItem;
        itemIcon.sprite = item.icon;
        cooldown = item.cooldownTime;
        effectTime = item.effectDuration; // 重新设置生效时长
        currentTime = 0; // 道具初始化时无冷却
        isEffectActive = false; // 初始化时物品没有生效
    }

    #region 处理道具的使用

    private void UseItem()
    {
        if (item == null)
        {
            Debug.LogError("Item 未初始化！");
            return;
        }

        if (currentTime > 0 && !isEffectActive) // 如果冷却时间还没结束并且物品未生效，直接返回
        {
            Debug.Log($"道具 {item.itemName} 冷却中，还需 {currentTime:F1} 秒");
            return;
        }

        ApplyItemEffect();
        effectTime = item.effectDuration; // 设置生效时长
        isEffectActive = true; // 激活物品效果
        currentTime = 0; // 重置冷却时间（冷却时间将会在生效结束后重新开始）
    }

    // 触发道具技能效果
    private void ApplyItemEffect()
    {
        switch (item.effectType)
        {
            case ItemEffectType.slingshot:
                EVENTMGR.TriggerUsingSlingshot();
                break;
            case ItemEffectType.energyMedicine:
                player.UseEnergyMedicine();
                break;
        }
    }

    #endregion
}