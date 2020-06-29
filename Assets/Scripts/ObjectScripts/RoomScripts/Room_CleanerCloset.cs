using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;
using System.Linq;
using MySpace.Stats;

public class Room_CleanerCloset : Room_WorkQuarters
{
    /// <summary>
    /// Ratio at which this cleanercloset rests up the cleaners in order for them to get back to work. 
    /// Higher ratio is faster resting speed. The value affects cleaning rate linearly. 
    /// A restRatio of 2 doubles the cleaning speed of all cleaners, a rest ratio of .5f would halve the cleaning speed of all cleaners.
    /// </summary>
    public const float restRatio = 2;

    public override Enums.ManRole RoomRole => Enums.ManRole.Cleaner;

    public static event Action RoomFinishedCleaningEvent;

    public override Action<ManScript_Worker> GetRoleFunction => Cleaner;

    public override SpecialtyStat.StatType[] specialStatsUsed => new SpecialtyStat.StatType[1] { SpecialtyStat.StatType.Physicality };

    public override void RemoveManFromRoomSlot(Guid manId)
    {
        base.RemoveManFromRoomSlot(manId);
    }

    private static void Cleaner(ManScript_Worker man)
    {
        //Room is a bedroom
        if (man.ManData.AssignedRoom.RoomData.RoomType == Enums.RoomTypes.Bedroom)
        {
            (man.ManData.AssignedRoom as Room_Bedroom).CleanRoom(man.GetSpecialtyStatValue(MySpace.Stats.SpecialtyStat.StatType.Physicality));
            man.currentTiredness -= Time.deltaTime;
            if (man.currentTiredness < 0)
            {
                man.currentTiredness = 0;
                PathfindToClosestCloset(man);
            }
            else if ((man.ManData.AssignedRoom as Room_Bedroom).Cleanliness >= 1)
            {
                PathfindToDirtyRoom(man);
                if (GameManager.Debug)
                    print("man finished cleaning");
                RoomFinishedCleaningEvent?.Invoke();
            }
        }
        //Room is a cleaner closet
        else if (man.ManData.AssignedRoom.RoomData.RoomType == Enums.RoomTypes.CleanerCloset)
        {
            man.currentTiredness += Time.deltaTime * restRatio;
            if (man.currentTiredness > ManScript_Worker.tirednessMax)
            {
                man.currentTiredness = ManScript_Worker.tirednessMax;
                //Delay timer is here so that pathfinding isnt done multiple times per frame, 60 frames per second. that would be bad.
                if (man.delayTimer >= 2)
                {
                    PathfindToDirtyRoom(man);
                    if (GameManager.Debug)
                        print("man moving to clean a bedroom");
                    man.delayTimer = 0;
                }
                
                man.delayTimer += Time.deltaTime;
            }
        }
    }

    #region Pathfinding Helper Functions

    //Made these into separate functions because the logic that dictates how each room type is found will probably change and differ a lot in future development.
    static void PathfindToClosestCloset(ManScript_Worker man)
    {
        //This is where we will change how we find the room that the cleaner needs to go to.
        ManManager.Ref.MoveManToClosestRoomOfType<Room_CleanerCloset>(man);
    }

    static void PathfindToDirtyRoom(ManScript_Worker man)
    {
        //Below here is where we will change how we find the room that the cleaner needs to go to.

        //If we want the worker to not prioritize empty rooms we can simply move this code below the second search.
        Room_Bedroom chosen = null;
        float dirtiest;
        IEnumerable<RoomScript> ar;

        dirtiest = 1;
        ar = (from r in RoomManager.Ref.GetAllActiveRoomsofType(Enums.RoomTypes.Bedroom) where (r.RoomScript is Room_Bedroom && !r.RoomScript.HasOwner()) select r.RoomScript);
        foreach (Room_Bedroom room in ar)
        {
            if (room.Cleanliness < dirtiest)
            {
                dirtiest = room.Cleanliness;
                chosen = room;
            }
        }

        if (chosen != null)
            goto skip;

        dirtiest = Room_Bedroom.cleanlinessThreshhold;
        ar = (from r in RoomManager.Ref.GetAllActiveRoomsofType(Enums.RoomTypes.Bedroom) where (r.RoomScript is Room_Bedroom && /*for now, this will let cleaners and guests be in bedrooms at the same time. Might want to change this later.*/r.RoomScript.RoomHasFreeManSlots()) select r.RoomScript);
        foreach (Room_Bedroom room in ar)
        {
            if (room.Cleanliness < dirtiest)
            {
                dirtiest = room.Cleanliness;
                chosen = room;
            }
        }

        skip:
        if (chosen != null)
            ManManager.Ref.MoveManToNewRoom(man.ManData.ManId, chosen.RoomData.RoomId);
        else if (man.ManData.AssignedRoom.RoomData.RoomType != Enums.RoomTypes.CleanerCloset)
            PathfindToClosestCloset(man);
    }
    #endregion
}
