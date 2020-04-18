using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;
using System.Linq;

public class Room_CleanerCloset : Room_WorkQuarters
{
    /// <summary>
    /// Ratio at which this cleanercloset rests up the cleaners in order for them to get back to work. Higher ratio is faster resting speed.
    /// </summary>
    public float restRatio = 2;

    public override Enums.ManRole RoomRole => Enums.ManRole.Cleaner;

    public static event Action RoomFinishedCleaningEvent;

    public override Action<ManScript_Worker> GetRoleFunction => Cleaner;

    public override void RemoveManFromRoomSlot(Guid manId)
    {
        base.RemoveManFromRoomSlot(manId);
    }

    private void Cleaner(ManScript_Worker man)
    {
        //Room is a bedroom
        if (man.ManData.AssignedRoom.RoomData.RoomType == Enums.RoomTypes.Bedroom)
        {
            man.currentTiredness -= Time.deltaTime;
            if (man.currentTiredness < 0)
            {
                man.currentTiredness = 0;
                PathfindToClosestCloset(man);
            }
            else if ((man.ManData.AssignedRoom as Room_Bedroom).Cleanliness >= 1)
            {
                PathfindToClosestCloset(man);
                RoomFinishedCleaningEvent?.Invoke();
            }
        }
        //Room is a cleaner closet
        else if (man.ManData.AssignedRoom.RoomData.RoomType == Enums.RoomTypes.CleanerCloset)
        {
            man.currentTiredness += Time.deltaTime * restRatio;
            if (man.currentTiredness > man.tirednessThreshhold)
            {
                man.currentTiredness = man.tirednessThreshhold;
                //Delay timer is here so that pathfinding isnt done multiple times per frame, 60 frames per second. that would be bad.
                if (man.delayTimer >= 2)
                {
                    PathfindToDirtyRoom(man);
                    man.delayTimer = 0;
                }
                
                man.delayTimer += Time.deltaTime;
            }
        }
    }

    #region Pathfinding Helper Functions

    //Made these into separate functions because the logic that dictates how each room type is found will probably change and differ a lot in future development.
    void PathfindToClosestCloset(ManScript_Worker man)
    {
        //This is where we will change how we find the room that the cleaner needs to go to.
        ManManager.Ref.MoveManToClosestRoomOfType<Room_CleanerCloset>(man);
    }

    void PathfindToDirtyRoom(ManScript_Worker man)
    {
        //Below here is where we will change how we find the room that the cleaner needs to go to.

        Room_Bedroom chosen = null;
        float dirtiest = Room_Bedroom.cleanlinessThreshhold;
        var ar = (from r in RoomManager.Ref.GetAllActiveRoomsofType(Enums.RoomTypes.Bedroom) where (r.RoomScript is Room_Bedroom && /*for now, this will let cleaners and guests be in bedrooms at the same time. Might want to change this later.*/r.RoomScript.RoomHasFreeManSlots()) select r.RoomScript);
        foreach (Room_Bedroom room in ar)
        {
            if (room.Cleanliness < dirtiest)
            {
                dirtiest = room.Cleanliness;
                chosen = room;
            }
        }

        if (chosen != null)
            ManManager.Ref.MoveManToNewRoom(man.ManData.ManId, chosen.RoomData.RoomId);
    }
    #endregion
}
