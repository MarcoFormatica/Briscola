using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnchor : MonoBehaviour
{
    public bool IsOccupied()
    {
        return GetComponentInChildren<Card>() != null;
    }

    public void PlaceCard(Card card)
    {
        card.gameObject.transform.parent = transform;

        card.transform.localPosition = Vector3.zero;    
        card.transform.localRotation = Quaternion.identity;
        card.GetComponent<NetworkTransform>().Teleport(transform.position, transform.rotation);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
    }

    internal Card GetCard()
    {
        return GetComponentInChildren<Card>();
    }
}
