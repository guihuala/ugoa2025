using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InteractableItemTrigger : MonoBehaviour, IEnterSpecialItem
{
    public BaseOfficeItem targetObject;

    private Vector3 originalScale = new Vector3(1, 1, 1);

    
    public void Apply()
    {
        targetObject.IsActive = true;
    }
}
