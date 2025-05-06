using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private List<DialogueTrigger> linkedTriggers; // 联动触发器列表
    private bool isPlayed = false;
    private bool isDisabledByLink = false; // 是否被其他联动触发器禁用

    public void Apply()
    {
        // 如果已经播放过或被其他触发器禁用，则返回
        if (isPlayed || isDisabledByLink)
            return;

        // 触发所有联动触发器，将它们标记为禁用
        foreach (var trigger in linkedTriggers)
        {
            if (trigger != this) // 避免自引用
            {
                trigger.DisableByLinkedTrigger();
            }
        }

        // 开始对话
        DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
        dialoguePanel.StartDialogue(dialogueData);
        
        isPlayed = true;
    }

    // 被其他联动触发器调用，禁用此触发器
    public void DisableByLinkedTrigger()
    {
        isDisabledByLink = true;
    }
}