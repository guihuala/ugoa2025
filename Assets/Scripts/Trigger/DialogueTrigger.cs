using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private List<DialogueTrigger> linkedTriggers; // 联动触发器列表
    [SerializeField] private PlayableDirector timeline; // 可选的Timeline引用
    
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

        // 如果有配置 Timeline，则播放
        if (timeline != null)
        {
            PlayTimelineAndStartDialogue();
        }
        else
        {
            StartDialogueDirectly();
        }
        
        isPlayed = true;
    }

    private void PlayTimelineAndStartDialogue()
    {
        timeline.Play();
        // 在Timeline播放完成后开始对话
        timeline.stopped += OnTimelineFinished;
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        director.stopped -= OnTimelineFinished;
        
        // 开始对话
        StartDialogueDirectly();
    }

    private void StartDialogueDirectly()
    {
        DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
        dialoguePanel.StartDialogue(dialogueData);
    }
    
    public void DisableByLinkedTrigger()
    {
        isDisabledByLink = true;
    }
}