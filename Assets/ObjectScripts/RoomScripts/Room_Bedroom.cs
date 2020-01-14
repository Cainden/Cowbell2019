﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;

public class Room_Bedroom : RoomScript
{
    #region Serialized Variables
    [Tooltip("Amount of time between each tick of cleanliness increase/decrease")]
    public float cleanTickTime = 2;

    #endregion

    #region Private Variables
    MeshRenderer thisRend;

    private float fElapsedTime = 0;

    // Room Attributes
    private float cleanliness;

    //Here as a fillable reference instead of creating a local reference-type variable each frame of update
    ManRef occupant;
    #endregion

    protected override void Start()
    {
        base.Start();
        //thisRend = obj.GetComponent<Renderer>();
        thisRend = GetComponentInChildren<MeshRenderer>();
        thisRend.material.color = Color.white;
        cleanliness = 1.0f;
    }

    
    protected override void Update()
    {
        base.Update();

        fElapsedTime += Time.deltaTime;
        if (RoomIsActive && fElapsedTime > cleanTickTime )
        {
            int numMen = CountMen();
            fElapsedTime = 0.0f;
            bool bStank = false; // If true deteriorates room
            bool bClean = false; // If true cleans room. 
            int numGuests = 0, numCleaners = 0;
            //Check to see if Room is occupied by Guest and decrease cleanliness. 
            //If room is occupied by Cleaners, increase cleanliness.
            //Make it so Cleaners cannot occupy the rooms at the same time as guests?

            for (int i = 0; i < numMen; i++)
            {
                if (RoomData.ManSlotsAssignments[i] == Guid.Empty)
                    continue;

                //Moved this to a field instead of a local variable to save cpu since it was in update
                occupant = manManRef.GetManData(RoomData.ManSlotsAssignments[i]);

                if (occupant.ManScript.ManData.ManType == Enums.ManTypes.Guest && occupant.ManScript.State == Enums.ManStates.None)
                {
                    bStank = true;
                    numGuests += 1;
                }
                else if (occupant.ManScript.ManData.ManType == Enums.ManTypes.Cleaner && occupant.ManScript.State == Enums.ManStates.None)
                {
                    bClean = true;
                    numCleaners += 1;
                }
                occupant = null;
            }


            if (bStank)
            {

                if (cleanliness - RoomManager.Ref.DirtinessSpeedRatio * numGuests < 0.0f)
                    cleanliness = 0.0f;
                else
                {
                    cleanliness -= RoomManager.Ref.DirtinessSpeedRatio * numGuests;
                }
            }
            else if (bClean)
            {

                if (cleanliness + RoomManager.Ref.CleanSpeedRatio * numCleaners > 1.0f)
                    cleanliness = 1.0f;
                else
                {
                    cleanliness += RoomManager.Ref.CleanSpeedRatio * numCleaners;
                }
            }



            thisRend.material.SetColor("_Color", Color.Lerp(Color.green, Color.white, cleanliness));
        }
    }

    public void SelfInitialize(GridIndex leftMostIndex)
    {
        //HAD TO DO ALL OF THE CREATEROOM INITIALIZATION HERE BECAUSE IT CANNOT BE INSTANTIATED IN THE WRONG POSITION THROUGH CREATEROOM
        RoomDefData RoomDefData = new RoomDefData("BedroomBase", gameObject, Enums.RoomSizes.Size2, Enums.RoomTypes.Bedroom_Size2, Enums.RoomCategories.Miscellaneous, 2, CreateNewArray(2), "Base Bedroom", 0, Enums.RoomOverUnder.Over, false);

        RoomData = new RoomInstanceData();
        RoomData.RoomId = Guid.NewGuid();
        RoomData.RoomName = RoomDefData.RoomName;
        RoomData.RoomSize = RoomDefData.RoomSize;
        RoomData.RoomCategory = RoomDefData.RoomCategory;
        RoomData.RoomType = RoomDefData.RoomType;
        RoomData.RoomOverUnder = RoomDefData.RoomOverUnder;
        RoomData.ManSlotCount = RoomDefData.ManSlotCount;
        RoomData.ManSlotsPositions = new Vector3[RoomDefData.ManSlotCount]; // Data will be set by object script on Start()
        RoomData.ManSlotsRotations = new Quaternion[RoomDefData.ManSlotCount]; // Data will be set by object script on Start()
        RoomData.ManSlotsAssignments = new Guid[RoomDefData.ManSlotCount];
        RoomData.OwnerSlotsAssignments = new Guid[RoomDefData.ManSlotCount];

        for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.ManSlotsAssignments[i] = Guid.Empty;
        RoomData.ManWorkingStates = RoomDefData.ManWorkingStates;
        RoomData.CoveredIndizes = GridManager.Ref.GetOccupiedindizes(RoomDefData.RoomSize, leftMostIndex);

        for (int i = 0; i < RoomData.ManSlotCount; i++) RoomData.OwnerSlotsAssignments[i] = Guid.Empty;

        GridManager.Ref.RegisterAtGrid(RoomData.RoomSize, RoomData.RoomId, leftMostIndex);
        RoomManager.Ref.AddRoom(RoomData.RoomId, new RoomRef(gameObject, this));

        #region Manual assignment of grid index movement directions
        //enable the  way to leave the room through the door
        GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[RoomData.CoveredIndizes.Length - 1], Enums.MoveDirections.Front);
        //enable left movement to the door index
        GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[RoomData.CoveredIndizes.Length - 1], Enums.MoveDirections.Left);
        //enable right movement to the index inside the room
        GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[0], Enums.MoveDirections.Right);
        #endregion

        Enums.ManStates[] CreateNewArray(int length)
        {
            var ar = new Enums.ManStates[length];
            for (int i = 0; i < length; i++)
            {
                ar[i] = Enums.ManStates.Idle;
            }
            return ar;
        }
    }
}
