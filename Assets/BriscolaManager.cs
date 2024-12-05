using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BriscolaManager : NetworkBehaviour
{

    public List<SerializableCard> deck;
    public NetworkObject cardPrefab;
    public NetworkObject playerBoardPrefab;
    public SerializableCard briscola;
    public List<EPlayerType> turnOrder;


    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {
            SetupBriscola();
        }
    }

    private void SetupBriscola()
    {
        CreateDeck();

        ExtractBriscola();

        SetupPlayerBoards();

        SetupFirstTurnOrder();
    }

    private void SetupFirstTurnOrder()
    {
        turnOrder = GetPlayerTypeList();
    }

    private static List<EPlayerType> GetPlayerTypeList()
    {
        return new List<EPlayerType>((EPlayerType[])Enum.GetValues(typeof(EPlayerType))).FindAll(x => x != EPlayerType.None);
    }

    private void SetupPlayerBoards()
    {
        foreach (EPlayerType type in GetPlayerTypeList())
        {
            NetworkObject playerBoard = Runner.Spawn(playerBoardPrefab);
            playerBoard.GetComponent<PlayerBoard>().PlayerOwner = type;

            for (int i = 1; i <= 3; i++)
            {
                playerBoard.GetComponent<PlayerBoard>().AddCard(DrawCard(deck));
            }
        }
    }

    private void ExtractBriscola()
    {
        briscola = deck[deck.Count - 1];
        NetworkObject briscolaNO = SpawnCard(briscola);

    }

    private void CreateDeck()
    {
        deck = new List<SerializableCard>();
        for (int i = 1; i <= 10; i++)
        {
            foreach (ESeed seed in Enum.GetValues(typeof(ESeed)))
            {
                if (seed != ESeed.None)
                {
                    deck.Add(new SerializableCard(seed, i));
                }
            }
        }
    }

    private Card DrawCard(List<SerializableCard> deck)
    {
        SerializableCard drawedCard = deck[0];
        deck.Remove(drawedCard);
        return SpawnCard(drawedCard).GetComponent<Card>();
    }

    private NetworkObject SpawnCard(SerializableCard serializableCard)
    {
        NetworkObject spawnedCard = Runner.Spawn(cardPrefab);
        spawnedCard.GetComponent<Card>().Seed = serializableCard.seed;
        spawnedCard.GetComponent<Card>().Number = serializableCard.number;
        return spawnedCard;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayCard(ESeed seed, int number, EPlayerType player) 
    {

    }

}
