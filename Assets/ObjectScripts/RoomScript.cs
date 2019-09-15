// This script is attached to the rooms

using MySpace;
using System;
using UnityEngine;
using System.Collections.Generic;

public class RoomScript : MonoBehaviour
{
    // Instance specific data. Kept separate for easier serialization
    public RoomInstanceData RoomData;
    MeshRenderer thisRend;
   

    // Internals
    public bool RoomIsActive { get; private set; } // True if at least one man working here
    private float fElapsedTime = 0;

    // Room Attributes
    public float cleanliness;


    ManManager manManRef;


    public void IncreaseCleanliness(float num)
    {
        // may not be necessary
    }

    private void Start()
    {
        AssignManSlotPositions();
        CheckReferences();
        SetRoomText();
        //thisRend = obj.GetComponent<Renderer>();
        thisRend = GetComponentInChildren<MeshRenderer>();
        thisRend.material.color = Color.white;
        cleanliness = 1.0f;
        manManRef = ManManager.Ref;
    }


    private void Update()
    {
        //May be better in a coroutine method, look into this later
        if (RoomData.RoomType == Enums.RoomTypes.Bedroom)
        {
            fElapsedTime += Time.deltaTime;
            if (RoomIsActive && fElapsedTime > 2.0f)
            {
                int numMen = CountMen();
                fElapsedTime = 0.0f;
                bool bStank = false; // If true deteriorates room
                bool bClean = false; // If true cleans room. 
                float numGuests = 0;
                float numCleaners = 0;
                //Check to see if Room is occupied by Guest and decrease cleanliness. 
                //If room is occupied by Cleaners, increase cleanliness.
                //Make it so Cleaners cannot occupy the rooms at the same time as guests?

                for (int i = 0; i < numMen; i++)
                {
                    if (RoomData.ManSlotsAssignments[i] == Guid.Empty)
                        continue;

                    if(manManRef.GetManData(RoomData.ManSlotsAssignments[i]).ManScript.ManData.ManType
                    == Enums.ManTypes.Guest)
                    {
                        bStank = true;
                        numGuests += 1.0f;
                    }
                    else if(manManRef.GetManData(RoomData.ManSlotsAssignments[i]).ManScript.ManData.ManType
                     == Enums.ManTypes.Cleaner)
                    {
                        bClean = true;
                        numCleaners += 1.0f;
                    }
                }


                if (bStank)
                {

                    if (cleanliness - 0.1f * numGuests < 0.0f)
                        cleanliness = 0.0f;
                    else
                    {
                        cleanliness -= 0.1f * numGuests;
                    }
                }
                else if (bClean)
                {

                    if (cleanliness + 0.1f * numCleaners > 1.0f)
                        cleanliness = 1.0f;
                    else
                    {
                        cleanliness += 0.1f * numCleaners;
                    }
                }



                thisRend.material.SetColor("_Color", Color.Lerp(Color.green, Color.white, cleanliness));
            }
        }
    }

    private void CheckReferences()
    {
        Debug.Assert(RoomData != null);
        Debug.Assert(RoomData.ManSlotCount == RoomData.ManSlotsPositions.Length);
        Debug.Assert(RoomData.ManSlotCount == RoomData.ManSlotsRotations.Length);
        Debug.Assert(RoomData.ManSlotCount == RoomData.ManSlotsAssignments.Length);
    }

    private void AssignManSlotPositions() // Get men positions from model
    {
        Transform[] Children = GetComponentsInChildren<Transform>();

        for (int i = 0; i < Children.Length; i++)
        {
            string SlotName = "SlotPos" + (i + 1).ToString();
            foreach (Transform Child in Children)
            {
                if (Child.name == SlotName)
                {
                    RoomData.ManSlotsPositions[i] = Child.position;
                    RoomData.ManSlotsRotations[i] = Child.rotation;
                }
            }
        }
    }

    public bool RoomHasFreeManSlots()
    {
        foreach (Guid g in RoomData.ManSlotsAssignments)
        {
            if (g == Guid.Empty) return (true);
        }
        return (false);
    }


    public bool AllManSlotsAreEmpty()
    {
        foreach (Guid g in RoomData.ManSlotsAssignments)
        {
            if (g != Guid.Empty) return (false);
        }
        return (true);
    }

    public int GetFreeManSlotIndex()
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ManSlotsAssignments[i] == Guid.Empty)
            {
                return (i);
            }
        }
        return (-1);
    }

    public bool RoomContainsMan(Guid manId)
    {
        foreach (Guid g in RoomData.ManSlotsAssignments)
        {
            if (g == manId) return (true);
        }
        return (false);
    }

    public int CountMen()
    {
        int Count = 0;
        foreach (Guid g in RoomData.ManSlotsAssignments)
        {
            if (g != Guid.Empty) Count++;
        }
        return (Count);
    }

    public void AssignManToRoomSlot(Guid manId, int slotIndex)
    {
        RoomData.ManSlotsAssignments[slotIndex] = manId;
        RoomIsActive = true; // General sign that there is a worker
    }

    public void RemoveManFromRoomSlot(Guid manId)
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.ManSlotsAssignments[i] == manId) RoomData.ManSlotsAssignments[i] = Guid.Empty;
        }

        if (AllManSlotsAreEmpty()) RoomIsActive = false;
    }

    // ROOM OWNER CODE /////////////
    public bool RoomHasFreeOwnerSlots()
    {
        foreach (Guid g in RoomData.OwnerSlotsAssignments)
        {
            if (g == Guid.Empty) return (true);
        }
        return (false);
    }

    public bool AllOwnerSlotsAreEmpty()
    {
        foreach (Guid g in RoomData.OwnerSlotsAssignments)
        {
            if (g != Guid.Empty) return (false);
        }
        return (true);
    }

    public int GetFreeOwnerSlotIndex()
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.OwnerSlotsAssignments[i] == Guid.Empty)
            {
                return (i);
            }
        }
        return (-1);
    }

    public void AssignOwnerToRoomSlot(Guid manId, int slotIndex)
    {
        RoomData.OwnerSlotsAssignments[slotIndex] = manId;
        //RoomIsActive = true; // General sign that there is a worker
    }

    public void RemoveOwnerFromRoomSlot(Guid manId)
    {
        for (int i = 0; i < RoomData.ManSlotCount; i++)
        {
            if (RoomData.OwnerSlotsAssignments[i] == manId) RoomData.OwnerSlotsAssignments[i] = Guid.Empty;
        }

        //if (AllOwnerSlotsAreEmpty()) RoomIsActive = false;
    }

    private void SetRoomText()
    {
        Transform[] Children = GetComponentsInChildren<Transform>();

        foreach (Transform Child in Children)
        {
            if (Child.name == "TextPos")
            {
                GameObject RoomText = Instantiate(Resources.Load<GameObject>("RoomText"));
                //  RoomText.GetComponent<TextMesh>().text = RoomData.RoomType.ToString() + " " + RoomData.RoomSize.ToString();
                RoomText.GetComponent<TextMesh>().text = RoomData.RoomName.ToString();
                RoomText.transform.position = Child.transform.position;
                RoomText.transform.SetParent(transform);
                return;
            }
        }
    }
}
