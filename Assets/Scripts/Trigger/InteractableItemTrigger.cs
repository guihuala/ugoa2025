using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InteractableItemTrigger : MonoBehaviour, IEnterSpecialItem
{
    public BaseOfficeItem targetObject;
    private PlayerInteractManager playerInteractManager;

    private void Awake()
    {
        playerInteractManager = FindObjectOfType<PlayerInteractManager>();
    }
    
    public void Apply()
    {
        playerInteractManager.UpdateOfficeItem(targetObject);
    }
}