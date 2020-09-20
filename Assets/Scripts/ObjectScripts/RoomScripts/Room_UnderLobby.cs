using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public class Room_UnderLobby : RoomScript
{
    [Tooltip("Parent gameobject that contains the monster teleporter for when monster mode starts.")]
    [SerializeField] GameObject MonsterPorter;

    public void BeginNightMode()
    {
        //Display Tarot Card UI/Hit selection menu
        //NightModeManager.Ref.EnableNightMode();
    }

    protected override void Start()
    {
        base.Start();
        MySpace.Events.EventManager.AddEventTriggerToGameTime(22, 0, 0, BeginNightMode, true);
    }
}
