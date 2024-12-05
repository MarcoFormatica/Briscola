using Fusion;
using System.Collections;
using System.Collections.Generic;
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

    [Networked] public ESeed Seed { get; set; }
    [Networked] public int Number { get; set; }


    public void RefreshCardRender()
    {

    }

    public int GetPoints()
    {

        return 0;

    }

    public int GetPower()
    {

        return 0;
    }
}
