using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueTrigger : MonoBehaviour, IEnterSpecialItem
{
    public enum DialogueTiming
    {
        AfterTimeline,
        BeforeTimeline
    }

    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private List<DialogueTrigger> linkedTriggers; // 联动触发器列表
    [SerializeField] private PlayableDirector timeline; // 可选的Timeline引用
    [SerializeField] private DialogueTiming dialogueTiming = DialogueTiming.AfterTimeline; // 对话触发时机
    
    private bool isPlayed = false;
    private bool isTimelinePlayed = false;
    private bool isDisabledByLink = false; // 是否被其他联动触发器禁用


    private void Start()
    {
        if (dialogueTiming == DialogueTiming.BeforeTimeline)
        {
            EVENTMGR.OnDialogueEnd += PlayTimeline;
        }
    }

    private void OnDestroy()
    {
        if (dialogueTiming == DialogueTiming.BeforeTimeline)
        {
            EVENTMGR.OnDialogueEnd -= PlayTimeline;
        }
    }

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

        // 根据选择的时机决定执行顺序
        if (timeline != null)
        {
            if (dialogueTiming == DialogueTiming.BeforeTimeline)
            {
                StartDialogueDirectly();
            }
            else
            {
                PlayTimelineAndStartDialogue();
            }
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
        
        if(dialoguePanel != null)
            dialoguePanel.StartDialogue(dialogueData);
    }

    private void PlayTimeline()
    {
        if (isTimelinePlayed) return;

        isTimelinePlayed = true;
        timeline.Play();
    }
    
    public void DisableByLinkedTrigger()
    {
        isDisabledByLink = true;
    }
}