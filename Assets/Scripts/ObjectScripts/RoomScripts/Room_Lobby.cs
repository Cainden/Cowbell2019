using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public class Room_Lobby : RoomScript
{
    public bool FindOpenBedroom(ManScript man)
    {
        //Get all bedroom rooms
        var ar = RoomManager.Ref.GetAllActiveRoomsofType(Enums.RoomTypes.Bedroom);
        for (int i = 0; i < ar.Length; i++)
        {
            if (!ar[i].RoomScript.HasOwner())
            {
                ManManager.Ref.MoveManToNewRoom(man.ManData.ManId, ar[i].RoomScript.RoomData.RoomId);
                ManManager.Ref.TransferOwnershipToRoom(man.ManData.ManId, ar[i].RoomScript.RoomData.RoomId);
                return true;
            }
        }

        GuiManager.Ref.Initiate_UserInfoSmall("New Guests are waiting on a bedroom!");
        return false;
    }
}
