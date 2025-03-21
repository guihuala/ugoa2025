using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image itemIcon;
    public Image cooldownIcon;
    
    private ItemData item;
    private Button itemButton;
    private PlayerItemEffect player;
    
    private float currentTime; // 当前冷却时间
    private float cooldown;    // 冷却时长

    private void Start()
    {
        player = FindObjectOfType<PlayerItemEffect>();
        itemButton = GetComponent<Button>(); // 获取Button组件
        itemIcon = transform.GetChild(0).GetComponent<Image>();

        if (item != null)
        {
            itemIcon.sprite = item.icon;
            cooldown = item.cooldownTime; // 设定冷却时间
        }

        itemButton.onClick.AddListener(() => UseItem());
    }

    private void Update()
    {
        UpdateCooldowns();
        UpdateButtonState();
    }

    // 更新冷却时间
    private void UpdateCooldowns()
    {
        if (currentTime > 0)
            currentTime -= Time.deltaTime;
        
        cooldownIcon.fillAmount = currentTime / cooldown;
    }

    // 更新按钮状态（冷却时禁用按钮）
    private void UpdateButtonState()
    {
        if (item != null)
        {
            itemButton.interactable = (currentTime <= 0);
        }
    }

    // 更新道具信息
    public void UpdateItemInfo(ItemData newItem)
    {
        this.item = newItem;
        itemIcon.sprite = item.icon;
        cooldown = item.cooldownTime;
        currentTime = 0; // 道具初始化时无冷却
    }

    #region 处理道具的使用

    private void UseItem()
    {
        if (item == null)
        {
            Debug.LogError("Item 未初始化！");
            return;
        }

        if (currentTime > 0)
        {
            Debug.Log($"道具 {item.itemName} 冷却中，还需 {currentTime:F1} 秒");
            return;
        }
        
        ApplyItemEffect();
        currentTime = cooldown; // 重置冷却时间
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
