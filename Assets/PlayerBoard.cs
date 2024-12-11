using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerBoard : NetworkBehaviour
{

    [Networked, OnChangedRender(nameof(OnPlayerOwnerChanged))]
    public EPlayerType PlayerOwner { get; set; }

    [Networked, OnChangedRender(nameof(OnPointsChanged))]
    public int Points { get; set; }


    public Transform hand;
    public CardAnchor playedCardAnchor;
    public TextMeshPro pointsTextMeshPro;


    public void OnPlayerOwnerChanged()
    {

    }

    public void OnPointsChanged()
    {
        pointsTextMeshPro.text = Points.ToString(); 
    }

    public void AddCard(Card card) 
    {
        hand.GetComponentsInChildren<CardAnchor>().ToList().Find(x=>x.IsOccupied()==false).PlaceCard(card);
    }

    internal void PlayCard(ESeed seed, int number)
    {
        Card card = GetCard(seed, number);
        playedCardAnchor.PlaceCard(card);
        card.RPC_ShowCard();
    }

    private Card GetCard(ESeed seed, int number)
    {
        return GetCardsInHand().Find(x => x.Seed == seed && x.Number == number);
    }

    internal void TakeAllCardsFromTheTable()
    {
        int pointsToGain = 0;
        foreach (PlayerBoard playerBoard in FindObjectsOfType<PlayerBoard>()) 
        {
            Card playedCard = playerBoard.playedCardAnchor.GetCard();
            pointsToGain = pointsToGain + GetCardPoints(playedCard.Number);
            Runner.Despawn(playedCard.GetComponent<NetworkObject>());
        }
        Points = Points + pointsToGain;
    }

    public List<Card> GetCardsInHand()
    {
        return hand.GetComponentsInChildren<Card>().ToList();
    }

    public int GetCardPoints(int number)
    {
        if (number == 1) { return 11; }
        if (number == 3) { return 10; }
        if (number == 8) { return 2; }
        if (number == 9) { return 3; }
        if (number == 10) { return 4; }

        return 0;
    }

    internal SerializableCard GetLastPlayedCard()
    {
        Card lastPlayedCard = GetComponentInChildren<PlayedCardSlot>().gameObject.GetComponentInChildren<Card>();
        return new SerializableCard(lastPlayedCard.Seed, lastPlayedCard.Number);
    }
}
