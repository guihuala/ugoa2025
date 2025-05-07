using UnityEngine;
using UnityEngine.Playables;


public class LockedCageBehavior : MonoBehaviour
{
    [SerializeField] private LockedBirdBehavior[] birds;
    [SerializeField] private PlayableDirector timelineDirector;

    private bool hasTriggered = false;

    public void UnlockCage()
    {
        if (hasTriggered) return;

        if (timelineDirector != null)
        {
            timelineDirector.stopped += OnTimelineStopped; // 注册事件
            timelineDirector.Play();
        }

        ChangeBirdState(true);
        hasTriggered = true;
    }

    private void ChangeBirdState(bool canMove)
    {
        foreach (var bird in birds)
        {
            bird.CanMove(canMove);
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        // 移除事件，防止重复绑定
        director.stopped -= OnTimelineStopped;

        // 销毁 Timeline 对象（或只是组件）
        Destroy(director.gameObject); // 或者 Destroy(director); 如果你只想移除组件
    }
}
