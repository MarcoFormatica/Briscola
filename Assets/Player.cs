using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerType
{
    None,
    Red,
    Green,
    Blue,
    Yellow
}

public class Player : NetworkBehaviour
{

    [Networked, OnChangedRender(nameof(OnPlayerTypeChanged))] 
    public EPlayerType PlayerType { get; set; }

    public void OnPlayerTypeChanged()
    {
        GetComponent<MeshRenderer>().material.color = FromPlayerTypeToColor(PlayerType);
    }



    private Color FromPlayerTypeToColor(EPlayerType playerType)
    {
        if(playerType == EPlayerType.Green) { return Color.green; } 
        if(playerType == EPlayerType.Blue) { return Color.blue; }
        if(playerType == EPlayerType.Yellow) { return Color.yellow; }
        if(playerType == EPlayerType.Red) { return Color.red; }

        return Color.black;
    }

    

}
