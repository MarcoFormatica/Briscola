using Fusion;
using System;
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
    public NetworkObject briscolaNO;
    public CardAnchor briscolaCardAnchor;
    public NetworkObject deckPreview;


    [Networked, OnChangedRender(nameof(RefreshTurnPlayer))] public EPlayerType TurnPlayer { get; set; }

    public void RefreshTurnPlayer()
    {
        FindObjectOfType<Table>().gameObject.GetComponent<MeshRenderer>().material.color = Player.FromPlayerTypeToColor(TurnPlayer);
    }

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

            playerBoard.transform.parent = FindObjectsOfType<PlayerBoardSpawnPoint>().ToList().Find(x => x.playerType == type).transform;
            playerBoard.transform.localPosition = Vector3.zero;
            playerBoard.transform.localRotation = Quaternion.identity;

            playerBoard.GetComponent<NetworkTransform>().Teleport(transform.position, transform.rotation);

            playerBoard.transform.localPosition = Vector3.zero;
            playerBoard.transform.localRotation = Quaternion.identity;

            for (int i = 1; i <= 3; i++)
            {
                playerBoard.GetComponent<PlayerBoard>().AddCard(DrawCard(deck));
            }
        }
    }

    private void ExtractBriscola()
    {
        briscola = deck[deck.Count - 1];
        briscolaNO = SpawnCard(briscola);
        briscolaCardAnchor.PlaceCard(briscolaNO.GetComponent<Card>());

    }
    public static void Shuffle(IList<SerializableCard> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0,n + 1);
            SerializableCard value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
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

        Shuffle(deck);
    }

    private Card DrawCard(List<SerializableCard> deck)
    {
        SerializableCard drawedCard = deck[0];
        deck.Remove(drawedCard);
        return SpawnCard(drawedCard).GetComponent<Card>();
    }

    private NetworkObject SpawnCard(SerializableCard serializableCard)
    {
        if (deck.Count == 0)
        {
            Runner.Despawn(deckPreview);
            return briscolaNO;
        }
        else
        {
            NetworkObject spawnedCard = Runner.Spawn(cardPrefab);
            spawnedCard.GetComponent<Card>().Seed = serializableCard.seed;
            spawnedCard.GetComponent<Card>().Number = serializableCard.number;
            return spawnedCard;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayCard(ESeed seed, int number, EPlayerType player)
    {
        if (turnOrder.Count == 0) { return; }

        if (player == turnOrder[0])
        {
            if (PlayerHasCard(player, seed, number))
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
        turnOrder = GetPlayerTypeList();

        while (turnOrder.First() != turnWinner)
        {
            EPlayerType first = turnOrder.First();
            turnOrder.Remove(first);
            turnOrder.Add(first);
        }

        turnOrderBackup = new List<EPlayerType>(turnOrder);

        // turnOrder.ForEach(p => GetPlayerBoard(p).AddCard(DrawCard(deck)));

        foreach (EPlayerType playerType in turnOrder)
        {
            GetPlayerBoard(playerType).AddCard(DrawCard(deck));
        }

    }

    private void DeclareWinner()
    {
        List<PlayerBoard> leaderBoard = new List<PlayerBoard>(FindObjectsOfType<PlayerBoard>()).OrderBy(p => p.Points).Reverse().ToList();

        Debug.Log("LeaderBoard:");
        leaderBoard.ForEach(p => Debug.Log(p.PlayerOwner.ToString() + " - Points: " + p.Points));


    }

    private bool CheckLastTurn()
    {
        return deck.Count == 0 && GetPlayerBoard(EPlayerType.Red).GetCardsInHand().Count == 0;
    }

    private bool ConfrontCards(SerializableCard firstCard, SerializableCard secondCard)
    {
        if (firstCard.seed == secondCard.seed)
        {
            return GetPower(firstCard) > GetPower(secondCard);
        }

        return secondCard.seed == briscola.seed;
    }


    public int GetPower(SerializableCard serializableCard)
    {
       if(serializableCard.number == 1) { return 12; }
       if(serializableCard.number == 3) { return 11; }
        return serializableCard.number;
    }

    private SerializableCard GetLastPlayedCard(EPlayerType player)
    {
        return GetPlayerBoard(player).GetLastPlayedCard();
       // return new SerializableCard(.LastCardPlayedSeed, GetPlayerBoard(player).LastCardPlayedNumber);
    }

    private PlayerBoard GetPlayerBoard(EPlayerType playerType)
    {
        return new List<PlayerBoard>(FindObjectsOfType<PlayerBoard>()).Find(x => x.PlayerOwner == playerType);
    }

    private bool PlayerHasCard(EPlayerType player, ESeed seed, int number)
    {
        return FindObjectsOfType<Card>().ToList().Exists(c => c.Number == number && c.Seed == seed && c.GetOwner() == player);
    }
}
