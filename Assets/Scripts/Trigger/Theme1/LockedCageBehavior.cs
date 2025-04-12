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
}