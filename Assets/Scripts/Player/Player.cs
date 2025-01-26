using System;
using UnityEngine;


public class Player : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PickupLayer"))
        {
            Debug.Log("拾取物品: " + other.name);
            other.gameObject.GetComponent<ICollectible>()?.OnCollect();
        }
        
    }
}