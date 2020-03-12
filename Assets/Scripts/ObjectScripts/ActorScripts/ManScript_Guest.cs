using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;

public class ManScript_Guest : ManScript
{
    public float dirtyFactor = 1;

    public override Enums.ManTypes ManType { get { return Enums.ManTypes.Guest; } }

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
        SendSignalToLobby();
    }

    protected override void Update()
    {
        base.Update();
        CheckIfRentTime();
    }

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
            ManManager.Ref.MoveManToNewRoom(ManData.ManId, RoomManager.lobbyId);
            TransferOwnershipToNewRoom(RoomManager.lobbyId);
        }
        else
            BuildManager.BuildFinishedEvent -= SendSignalToLobby;
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
        if (IsOwnerOfRoom() && !hasPaidRent && TimeManager.Ref.worldTimeHour == 8)
        {
            PayUserInHoots("Rent", (int)ManData.OwnedRoomRef.RoomSize * 50);
            hasPaidRent = true;
        }
        else if (TimeManager.Ref.worldTimeHour != 8)
        {
            hasPaidRent = false;
        }
    }
    #endregion
}
