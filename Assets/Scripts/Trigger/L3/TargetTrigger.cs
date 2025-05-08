using System;
using UnityEngine;
using UnityEngine.Playables;

public class TargetTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private GameObject target;
    [SerializeField] private PlayableDirector timelineDirector;

    private bool isTriggered = false;
    private bool isActive = false;

    private void Start()
    {
        EVENTMGR.OnDialogueEnd += SetPlayerTarget;

        if (timelineDirector != null)
        {
            timelineDirector.stopped += OnTimelineFinished;
        }
    }

    private void OnDestroy()
    {
        EVENTMGR.OnDialogueEnd -= SetPlayerTarget;

        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        if (isTriggered) return;
        if (!isActive) return;

        isTriggered = true;
        EVENTMGR.TriggerEnterTargetField(target.transform.position);
    }

    private void SetPlayerTarget()
    {
        if (isTriggered) return;
        if (!isActive) return;

        isTriggered = true;
        EVENTMGR.TriggerEnterTargetField(target.transform.position);
    }

    public void Apply()
    {
        isActive = true;

        if (timelineDirector != null)
        {
            timelineDirector.Play();
        }
    }
}