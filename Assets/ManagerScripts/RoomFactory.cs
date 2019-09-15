// Managing different room versions.

using MySpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFactory : MonoBehaviour
{
    [HideInInspector]
    public static RoomFactory Ref { get; private set; } // For external access of script

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<RoomFactory>();
    }

    public GameObject CreateRoom(Enums.RoomTypes roomType, Enums.RoomSizes roomSize, Enums.RoomOverUnder roomOverUnder)
    {
        RoomDefData DefData = GetRoomDefData(roomType, roomSize, roomOverUnder );
        GameObject RoomObject = LoadRoom(DefData);
        RoomObject.SetActive(true);        
        return (RoomObject);
    }

    private GameObject LoadRoom(RoomDefData defData)
    {
        return (Instantiate(Resources.Load<GameObject>(defData.RoomModelFile)));        
    }

    public RoomDefData GetRoomDefData(Enums.RoomTypes roomType, Enums.RoomSizes roomSize, Enums.RoomOverUnder roomOverUnder)
    {
        foreach (RoomDefData DefData in Constants.RoomDefinitions)
        {
            if ((DefData.RoomType == roomType) && (DefData.RoomSize == roomSize) && (DefData.RoomOverUnder == roomOverUnder))
            {
                return (DefData);
            }
        }

        Debug.Assert(1 == 0);
        return (Constants.RoomDefinitions[0]);
    }
}
