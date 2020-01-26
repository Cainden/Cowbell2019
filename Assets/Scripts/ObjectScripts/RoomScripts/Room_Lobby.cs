using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public class Room_Lobby : RoomScript
{
    public void FindOpenBedroom(ManScript man)
    {
        //Get all bedroom rooms
        var ar = RoomManager.Ref.GetAllActiveRoomsofType(Enums.RoomTypes.Bedroom_Size2, Enums.RoomTypes.Bedroom_Size4, Enums.RoomTypes.Bedroom_Size6);
        for (int i = 0; i < ar.Length; i++)
        {
            if (ar[i].RoomScript.RoomHasFreeManSlots())
            {
                ManManager.Ref.MoveManToNewRoom(man.ManData.ManId, ar[i].RoomScript.RoomData.RoomId);
                return;
            }
        }

        //TEMPORARY
        //This needs to be extended upon later, only used as a debug display now
        GuiManager.Ref.Initiate_UserInfoSmall("New Guest has no bedroom!");
    }
}
