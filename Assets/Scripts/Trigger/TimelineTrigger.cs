using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour, IEnterSpecialItem
{
    public PlayableDirector director;
    
    private bool isPlayed;
    
    public void Apply()
    {
        if (!isPlayed)
        {
            director.Play();
            isPlayed = true;
        }
    }
}
