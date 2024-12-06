using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {
            Invoke(nameof(PickPlayerType), 0.5f);
        }
        OnPlayerTypeChanged();
    }


    public void PickPlayerType()
    {
        List<Player> playerList = new List<Player>(FindObjectsOfType<Player>());
        List<EPlayerType> types = new List<EPlayerType>((EPlayerType[])Enum.GetValues(typeof(EPlayerType)));

        PlayerType = types.Find( t => (t!=EPlayerType.None) && !playerList.Exists(p=>p.PlayerType == t));

        transform.parent = FindObjectsOfType<PlayerBoard>().ToList().Find(x => x.PlayerOwner == PlayerType).GetComponentInChildren<PlayerAnchor>().transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }


    private void Update()
    {
        if (HasStateAuthority)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if(Physics.Raycast(ray,out raycastHit))
                {
                    Card card = raycastHit.collider.GetComponentInParent<Card>();
                    if (card != null) 
                    {
                        FindObjectOfType<BriscolaManager>().RPC_PlayCard(card.Seed,card.Number,PlayerType);
                    }
                }
            }
        }
    }
}
