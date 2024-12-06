using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBoard : NetworkBehaviour
{

    [Networked, OnChangedRender(nameof(OnPlayerOwnerChanged))]
    public EPlayerType PlayerOwner { get; set; }

    [Networked, OnChangedRender(nameof(OnPointsChanged))]
    public int Points { get; set; }

    [Networked, OnChangedRender(nameof(OnLastCardPlayedNumberChanged))]
    public int LastCardPlayedNumber { get; set; }

    [Networked, OnChangedRender(nameof(OnLastCardPlayedSeedChanged))]
    public ESeed LastCardPlayedSeed { get; set; }

    public Transform hand;
    public CardAnchor playedCardAnchor;
    public void OnLastCardPlayedNumberChanged()
    {

    }
    public void OnLastCardPlayedSeedChanged()
    {

    }
    public void OnPlayerOwnerChanged()
    {

    }

    public void OnPointsChanged()
    {

    }

    public void AddCard(Card card) 
    {
        card.CardOwner = PlayerOwner;
        hand.GetComponentsInChildren<CardAnchor>().ToList().Find(x=>x.IsOccupied()==false).PlaceCard(card);
    }

    internal void PlayCard(ESeed seed, int number)
    {
        Card card = GetCard(seed, number);
        playedCardAnchor.PlaceCard(card);
        LastCardPlayedNumber = number;
        LastCardPlayedSeed = seed;
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

}
