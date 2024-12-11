using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum ESeed
{
    None,
    Bastoni,
    Spade,
    Coppe,
    Denari
}

[Serializable]
public class SerializableCard
{
    public ESeed seed;
    public int number;

    public SerializableCard(ESeed seed, int number)
    {
        this.seed = seed;
        this.number = number;
    }
}

public class Card : NetworkBehaviour
{
    public MeshRenderer meshRendererFront;
    public List<Material> materials;

    [Networked, OnChangedRender(nameof(RefreshCardRender))] public ESeed Seed { get; set; }
    [Networked, OnChangedRender(nameof(RefreshCardRender))] public int Number { get; set; }

    GameObject grabbedObject;

    public EPlayerType GetOwner() 
    {
        PlayerBoard playerBoard = gameObject.GetComponentInParent<PlayerBoard>();
        if (playerBoard == null)
        {
            return EPlayerType.None;
        }
        return playerBoard.PlayerOwner;
    }

    public bool IsPlayed()
    {
        return GetComponentInParent<PlayedCardSlot>() != null;
    }

    public void RefreshCardVisibility()
    {
        EPlayerType localPlayerType = FindObjectOfType<MultiplayerManager>().localPlayer.PlayerType;
        EPlayerType cardOwnerType = GetOwner();
        if(IsPlayed() || localPlayerType == cardOwnerType || cardOwnerType == EPlayerType.None)
        {
            meshRendererFront.gameObject.SetActive(false);
        }
    }

    public void RefreshCardRender()
    {
        int i = 0;
        if (Seed == ESeed.Bastoni) { i = 0; }
        if (Seed == ESeed.Spade) { i = 1; }
        if (Seed == ESeed.Coppe) { i = 2; }
        if (Seed == ESeed.Denari) { i = 3; }
        meshRendererFront.material = materials[i];

        GetComponentInChildren<TextMeshPro>().text = Number.ToString();
    }



    private void Awake()
    {
    }

    public override void Spawned()
    {
        base.Spawned();
        RefreshCardRender();
        Invoke(nameof(RefreshCardVisibility), 0.2f);
    }
}
