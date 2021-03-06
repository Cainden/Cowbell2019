﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;
using System.Linq;

public class ManScript_Guest : ManScript
{
    public override Enums.ManTypes ManType { get { return Enums.ManTypes.Guest; } }

    public override float GetNetRevenueCalculation
    {
        get
        {
            if (ManData.OwnedRoomRef?.RoomType != Enums.RoomTypes.Bedroom)// ? is there so that if the man does not have an assigned room they return no rent change, instead of generating a NRE
                return 0;
            return (ManData.OwnedRoomRef.RoomScript as Room_Bedroom).RentCost;
        }
    }

    public override RevenueInfo.RevenueType RevenueType => RevenueInfo.RevenueType.Guest;

    public override void SetOwnerOfRoom(Guid assignedRoom)
    {
        base.SetOwnerOfRoom(assignedRoom);

        //Had to copy this from the base because returning in the base function doesnt exit this one
        if (assignedRoom == Guid.Empty) return;

        RoomRef roomRefTemp = RoomManager.Ref.GetRoomData(assignedRoom);
        
        if (!IsOwnerOfRoom())
        {
            //if room has free Owner slot, set the room reference that the man has to the room we're trying to assign them to
            int freeSlot = roomRefTemp.RoomScript.GetFreeOwnerSlotIndex();
            if (freeSlot > -1)
            {
                roomRefTemp.RoomScript.RoomData.OwnerSlotsAssignments[freeSlot] = ManData.ManId;
                ManData.OwnedRoomRef = roomRefTemp.RoomScript.RoomData;
            }
        }
    }

    public override void TransferOwnershipToNewRoom(Guid newRoom)
    {
        base.TransferOwnershipToNewRoom(newRoom);
        RoomRef roomRefTemp = RoomManager.Ref.GetRoomData(newRoom);

        //Not sure why I needed it to be a bedroom before but I removed this check for now
        //if ((roomRefTemp.RoomScript is Room_Bedroom))
        //{
            
        //}

        //if room has free Owner slot, set the room reference that the man has to the room we're trying to assign them to
        int freeSlot = roomRefTemp.RoomScript.GetFreeOwnerSlotIndex();
        if (freeSlot > -1)
        {
            roomRefTemp.RoomScript.RemoveOwnerFromRoomSlot(ManData.ManId);
            roomRefTemp.RoomScript.RoomData.OwnerSlotsAssignments[freeSlot] = ManData.ManId;
            ManData.OwnedRoomRef = roomRefTemp.RoomScript.RoomData;
        }
    }

    protected override void Start()
    {
        base.Start();
        //ManManager.Ref.MoveManToNewRoom(ManData.ManId, RoomManager.lobbyId);
        //TransferOwnershipToNewRoom(RoomManager.lobbyId);
        //Add_Action_ToList(new ActionData(SendSignalToLobby));
        waiting = false;
        //SendSignalToLobby();
        //StartCoroutine(MoveToLobby(GameManager.StartPath)); //This has been moved to the base.Start()

        //I don't want to make this a time-based event, because it should be able to be reduced depending on the guest's experience at the hootel.
        stayTime = GameManager.GetRandomizedGuestStayTime();
    }

    protected override void Update()
    {
        base.Update();
        CheckIfRentTime();
        if (State == Enums.ManStates.None)
        {
            ManData.AssignedRoom?.GuestBehavior(this);
        }
    }

    protected override void LobbyMoveFinished()
    {
        SendSignalToLobby();
    }

    bool waiting;
    private void SendSignalToLobby()
    {
        ////Find the lobby
        //Room_Lobby r = ManData.OwnedRoomRef.RoomScript as Room_Lobby;
        ////if the man is not in the lobby
        //if (r != null)
            
        //else
        //    BuildManager.BuildFinishedEvent -= SendSignalToLobby;
        if (!ManManager.FindOpenBedroomForMan(this))
        {
            BuildManager.BuildFinishedEvent -= SendSignalToLobby;
            BuildManager.BuildFinishedEvent += SendSignalToLobby;
            Room_CleanerCloset.RoomFinishedCleaningEvent -= SendSignalToLobby;
            Room_CleanerCloset.RoomFinishedCleaningEvent += SendSignalToLobby;
            //ManManager.Ref.MoveManToNewRoom(ManData.ManId, RoomManager.lobbyId);
            //TransferOwnershipToNewRoom(RoomManager.lobbyId);
            waiting = true;
            SetMood(Enums.ManMood.Sad1, true);
            //SetState(Enums.ManStates.Idle, 0);
        }
        else
        {
            BuildManager.BuildFinishedEvent -= SendSignalToLobby;
            Room_CleanerCloset.RoomFinishedCleaningEvent -= SendSignalToLobby;
            if (waiting)
            {
                ResolveMood(Enums.ManMood.Sad1);
                waiting = false;
            }
            else
            {
                //Make the guest happier if they didnt have to wait
                moodScript.ModifyMood(15);
            }
        }
    }

    public void FindEntertainment()
    {
        var rooms = RoomManager.Ref.GetAllActiveWorkQuartersOfCategory(Room_WorkQuarters.WorkQuartersType.Entertainment);
        if (rooms == null)
        {
            print("returned null array");
            if (moodScript.OverrideMood != Enums.ManMood.Bored)
                moodScript.DisplayMood(Enums.ManMood.Bored);
            return;
        }
        else if (rooms.Length == 0)
        {
            print("return array length of 0");
            if (moodScript.OverrideMood != Enums.ManMood.Bored)
                moodScript.DisplayMood(Enums.ManMood.Bored);
            return;
        }
        if (moodScript.OverrideMood == Enums.ManMood.Bored)
            moodScript.ResolveMood(Enums.ManMood.Bored);
        ManManager.Ref.MoveManToNewRoom(ManData.ManId, RoomManager.Ref.GetClosestRoom(rooms, ManData.AssignedRoom.RoomData.CoveredIndizes[ManData.AssignedRoomSlot]).RoomData.RoomId);
    }

    public override void AssignCharacterSpriteByCharacterType()
    {
        SetCharacterSprites(ManData.CharSprite);
        /*
        int max = 0;
        var types = (from CharacterSwaper.CharLabel c in Enum.GetValues(typeof(CharacterSwaper.CharLabel)) where CheckLabelName(c) select c).ToArray();

        int rand = UnityEngine.Random.Range(0, max); //number from 0-(the number of possible sprites)
        SetCharacterSprites(types[rand]);

        bool CheckLabelName(CharacterSwaper.CharLabel c)
        {
            string[] split = c.ToString().Split('_');

            //These are the three strings that guests will use
            if (split[0] == "Mars" || split[0] == "Venus" || split[0] == "Saturn")
            {
                max++;
                return true;
            }
            return false;
        }
        */
    }

    protected override void NightModeStart()
    {
        base.NightModeStart();
        if (!isClickable)
            return;
        transform.position = GridManager.Ref.GetWorldPositionFromGridIndex(ManData.OwnedRoomRef.RoomScript.RoomData.CoveredIndizes[ManData.OwnedRoomRef.RoomScript.GetFreeManSlotIndex(this)]);

        //Somehow need to make sure that the room that they are about to be assigned to has available slots
        ManManager.Ref.MoveManToNewRoom(ManData.ManId, ManData.OwnedRoomRef.RoomId);
        
        MovementPath.Clear();
        NextAction = null;
    }

    #region Rent Methods

    private bool hasPaidRent = false;

    public void PayUserInHoots(string reason, int amount)
    {
        OverheadTextManager.Ref.OverheadHoots(amount.ToString(), transform.position);
        WalletManager.AddHoots(amount);
    }

    private void CheckIfRentTime()
    {
        if (IsOwnerOfRoom() && !hasPaidRent && TimeManager.Ref.worldTimeHour == 8 && RoomManager.IsRoomOfType<Room_Bedroom>(ManData.OwnedRoomRef.RoomScript))
        {
            stayTime--;
            if (stayTime <= 0)
            {
                ClickManager.Ref.DeleteMan(this);
                hasPaidRent = true;
                return;
            }


            PayUserInHoots("Rent", (ManData.OwnedRoomRef.RoomScript as Room_Bedroom).RentCost);
            hasPaidRent = true;
        }
        else if (TimeManager.Ref.worldTimeHour != 8)
        {
            hasPaidRent = false;
        }
    }
    #endregion

    #region Guest Stay Time
    private int stayTime;

    public void LowerStayTime(int amount)
    {
        stayTime -= amount;
    }

    public void OverRideLeave()
    {
        ClickManager.Ref.DeleteMan(this);
    }

    #endregion
}
