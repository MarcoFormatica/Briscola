using Fusion;
using System;
using System.Collections.Generic;
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
    [Networked, OnChangedRender(nameof(SetCardVisible))] public EPlayerType CardOwner { get; set; }

    public void SetCardVisible()
    {
      //  throw new NotImplementedException();
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
    }
}
