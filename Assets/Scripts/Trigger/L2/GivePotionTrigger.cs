using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GivePotionTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private PlayableDirector timeline; // 可选的Timeline引用

    private bool isPlayed = false;


    public void Apply()
    {
        if (isPlayed) return;
        
        isPlayed = true;
        timeline.Play();
    }
}
