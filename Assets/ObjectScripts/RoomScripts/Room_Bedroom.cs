using System.Collections;
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

    public void IncreaseCleanliness(float num)
    {
        // may not be necessary
    }

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

                if (cleanliness - RoomManager.Ref.CleanSpeedRatio * numGuests < 0.0f)
                    cleanliness = 0.0f;
                else
                {
                    cleanliness -= RoomManager.Ref.CleanSpeedRatio * numGuests;
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
}
