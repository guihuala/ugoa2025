using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour, IEnterSpecialItem
{
    [SerializeField] private NodeMarker anotherNode;

    public void Apply()
    {
        if (anotherNode != null)
        {
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            playerMovement.Teleport(anotherNode.transform.position);
        }
    }
}
