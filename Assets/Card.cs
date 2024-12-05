using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ESeed
{
    Bastoni,
    Spade,
    Coppe,
    Denari
}
public class Card : NetworkBehaviour
{
    public MeshRenderer meshRendererFront;
    public List<Material> materials;
    [Networked] public ESeed Seed { get; set; }
    [Networked] public int Number { get; set; }


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

    public int GetPoints()
    {

        return 0;

    }

    public int GetPower()
    {

        return 0;
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
