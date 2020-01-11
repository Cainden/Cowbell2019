using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;

public class Room_Hallway : RoomScript
{
    const string BedroomPrefabPath = "Room_Prefabs/Room_BedroomBase";

    public Transform BedroomPosition;

    Room_Bedroom[] bedrooms;

    protected override void Start()
    {
        base.Start();
        bedrooms = new Room_Bedroom[(int)RoomData.RoomSize / 2];
        for (int i = 0; i < bedrooms.Length; i++)
        {
            bedrooms[i] = Instantiate(Resources.Load<GameObject>(BedroomPrefabPath), BedroomPosition.position + (Vector3.right * i * 5), BedroomPosition.rotation, transform).GetComponent<Room_Bedroom>();
            int g = Mathf.CeilToInt(i * 2);
            bedrooms[i].SelfInitialize(RoomData.CoveredIndizes[Mathf.CeilToInt(i * 2) + 1].GetBack().GetLeft());
            
        }

        for (int i = 1; i < RoomData.CoveredIndizes.Length; i+= 2)
        {
            GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[i], Enums.MoveDirections.Back);
        }
    }

    public Room_Bedroom GetBedroomFromX(float x)
    {
        //make sure there are bedrooms to highlight
        Debug.Assert(bedrooms.Length > 0);

        //offset the transform position relative to the leftmost object that can be hit on any hallway
        x += 1.25f;

        //prevent index out of range exception in edgecases of the mouse being perfectly on the far left object (resulting in an x value of about -.01 which will give an index of -1 when floored)
        if (x < 0)
            x = 0;

        return bedrooms[Mathf.FloorToInt(x / 5)];
    }
}
