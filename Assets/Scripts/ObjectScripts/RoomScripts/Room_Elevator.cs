using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MySpace;

public class Room_Elevator : RoomScript
{
    private List<Room_Elevator> eTower = null;

    public override void OnInitialization()
    {
        base.OnInitialization();

        Guid above = GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetAbove()), below = GridManager.Ref.GetGridTileRoomGuid(RoomData.CoveredIndizes[0].GetBelow());

        if (eTower != null) return;

        if (RoomManager.Ref.GetRoomData(below)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        {
            eTower = (RoomManager.Ref.GetRoomData(below).RoomScript as Room_Elevator).eTower;
            eTower.Add(this);

            //if both above and below
            if (RoomManager.Ref.GetRoomData(above)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
            {
                foreach (Room_Elevator room in (RoomManager.Ref.GetRoomData(above).RoomScript as Room_Elevator).eTower)
                {
                    //Add that room to this elevator's elevator list
                    eTower.Add(room);

                    //Have all of those rooms' lists become the total list
                    room.eTower = eTower;
                }
            }
        }
        else if (RoomManager.Ref.GetRoomData(above)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        {
            eTower = (RoomManager.Ref.GetRoomData(above).RoomScript as Room_Elevator).eTower;
            eTower.Add(this);
        }
        else if (RoomData.CoveredIndizes[0].Y > Constants.GridSurfaceY && below == Guid.Empty)
        {
            eTower = new List<Room_Elevator>() { this };
            Guid temp = Guid.NewGuid();
            RoomManager.Ref.CreateRoom(temp, Enums.RoomTypes.Elevator, RoomData.CoveredIndizes[0].GetBelow());
            eTower.Add(RoomManager.Ref.GetRoomData(temp).RoomScript as Room_Elevator);
        }
        else if (above == Guid.Empty)
        {
            eTower = new List<Room_Elevator>() { this };
            Guid temp = Guid.NewGuid();
            RoomManager.Ref.CreateRoom(temp, Enums.RoomTypes.Elevator, RoomData.CoveredIndizes[0].GetAbove());
            eTower.Add(RoomManager.Ref.GetRoomData(temp).RoomScript as Room_Elevator);
        }

        
    }
}
