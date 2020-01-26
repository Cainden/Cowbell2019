using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;

public class ManScript_Guest : ManScript
{
    public override Enums.ManTypes ManType { get { return Enums.ManTypes.Guest; } }

    public override void SetOwnerOfRoom(Guid assignedRoom)
    {
        base.SetOwnerOfRoom(assignedRoom);
        RoomRef roomRefTemp = RoomManager.Ref.GetRoomData(assignedRoom);
        if (!IsOwnerOfRoom())
        {
            //if room has free Owner slot, set the room reference that the man has to the room we're trying to assign them to
            int freeSlot = -1;
            if ((freeSlot = roomRefTemp.RoomScript.GetFreeOwnerSlotIndex()) > -1)
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
        int freeSlot = -1;
        if ((freeSlot = roomRefTemp.RoomScript.GetFreeOwnerSlotIndex()) > -1)
        {
            roomRefTemp.RoomScript.RemoveOwnerFromRoomSlot(ManData.ManId);
            roomRefTemp.RoomScript.RoomData.OwnerSlotsAssignments[freeSlot] = ManData.ManId;
            ManData.OwnedRoomRef = roomRefTemp.RoomScript.RoomData;
        }
    }

    protected override void Start()
    {
        base.Start();
        ManManager.Ref.MoveManToNewRoom(ManData.ManId, RoomManager.lobbyId);
        TransferOwnershipToNewRoom(RoomManager.lobbyId);
        Add_Action_ToList(new ActionData(SendSignalToLobby));
    }

    private void SendSignalToLobby()
    {
        Room_Lobby r = ManData.OwnedRoomRef.RoomScript as Room_Lobby;
        if (r != null)
            r.FindOpenBedroom(this);
    }
}
