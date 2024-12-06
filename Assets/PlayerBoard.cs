using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoard : MonoBehaviour
{

    [Networked, OnChangedRender(nameof(OnPlayerOwnerChanged))]
    public EPlayerType PlayerOwner { get; set; }

    [Networked, OnChangedRender(nameof(OnPointsChanged))]
    public int Points { get; set; }

    [Networked, OnChangedRender(nameof(OnLastCardPlayedNumberChanged))]
    public int LastCardPlayedNumber { get; set; }

    [Networked, OnChangedRender(nameof(OnLastCardPlayedSeedChanged))]
    public ESeed LastCardPlayedSeed { get; set; }

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
    }

    internal void PlayCard(ESeed seed, int number)
    {
        throw new NotImplementedException();
    }

    internal void TakeAllCardsFromTheTable()
    {
        throw new NotImplementedException();
    }

    public List<Card> GetCardsInHand()
    {
        throw new NotImplementedException();
    }
}