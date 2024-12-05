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
    public List<EPlayerType> turnOrderBackup;


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
        turnOrderBackup = new List<EPlayerType>(turnOrder);
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
        if(turnOrder.Count == 0) { return; }

        if (player == turnOrder[0]) 
        {
            if(PlayerHasCard(player, seed, number))
            {
                GetPlayerBoard(player).PlayCard(seed, number);
                turnOrder.RemoveAt(0);
                if (turnOrder.Count == 0)
                {
                    EPlayerType turnWinner = turnOrderBackup[0];
                    SerializableCard winnerCard = GetLastPlayedCard(turnWinner);

                    for (int i = 1; i < 4; i++)
                    {
                        EPlayerType candidate = turnOrderBackup[i];
                        SerializableCard candidateCard = GetLastPlayedCard(candidate);

                        if (!ConfrontCards(winnerCard, candidateCard)) 
                        {
                            winnerCard = candidateCard;
                            turnWinner = candidate;
                        }
                    }
                    GetPlayerBoard(turnWinner).TakeAllCardsFromTheTable();

                    if (CheckLastTurn())
                    {
                        DeclareWinner();
                    }
                    else
                    {
                        SetupNewTurn(turnWinner);
                    }

                }


            }
        }
    }

    private void SetupNewTurn(EPlayerType turnWinner)
    {
        throw new NotImplementedException();
    }

    private void DeclareWinner()
    {
        throw new NotImplementedException();
    }

    private bool CheckLastTurn()
    {
        throw new NotImplementedException();
    }

    private bool ConfrontCards(SerializableCard firstCard, SerializableCard secondCard)
    {
        if(firstCard.seed == secondCard.seed)
        {
            return GetPower(firstCard)>GetPower(secondCard);
        }

        return secondCard.seed == briscola.seed;
    }

    public int GetPoints(SerializableCard serializableCard)
    {

        return 0;

    }

    public int GetPower(SerializableCard serializableCard)
    {

        return 0;
    }

    private SerializableCard GetLastPlayedCard(EPlayerType player)
    {
        return new SerializableCard(GetPlayerBoard(player).LastCardPlayedSeed, GetPlayerBoard(player).LastCardPlayedNumber);
    }

    private PlayerBoard GetPlayerBoard(EPlayerType player)
    {
        throw new NotImplementedException();
    }

    private bool PlayerHasCard(EPlayerType player, ESeed seed, int number)
    {
        throw new NotImplementedException();
    }
}
