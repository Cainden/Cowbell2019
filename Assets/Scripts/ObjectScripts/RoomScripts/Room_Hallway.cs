using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using System;
using System.Linq;

public class Room_Hallway : RoomScript
{
    const string BedroomPrefabPath = "Room_Prefabs/Room_BedroomBase";

    public Transform BedroomPosition;

    Room_Bedroom[] bedrooms;

    public override void OnInitialization()
    {
        base.OnInitialization();
        bedrooms = new Room_Bedroom[(int)RoomData.RoomSize / 2];
        GameObject total;
        switch ((int)RoomData.RoomSize)
        {
            case 2:
                total = Instantiate(Resources.Load<GameObject>("Room_Prefabs/BedroomPrefabs/1UP_SINGLE_NOHALL_WithDoor_NAV"), BedroomPosition.position, BedroomPosition.rotation, transform);
                break;
            case 4:
                total = Instantiate(Resources.Load<GameObject>("Room_Prefabs/BedroomPrefabs/2UP_2Door_NOHALL_WithDoor_NAV"), BedroomPosition.position, BedroomPosition.rotation, transform);
                break;
            case 6:
                total = Instantiate(Resources.Load<GameObject>("Room_Prefabs/BedroomPrefabs/3up_3Door_NOHALL_WithDoor_NAV"), BedroomPosition.position, BedroomPosition.rotation, transform);
                break;
            default:
                Debug.LogError("Hallway is not a size 2, 4, or 6 room!");
                total = null;
                break;
        }

        //sort the bedrooms ascending by transform.x position
        bedrooms = (from Room_Bedroom room in total.GetComponentsInChildren<Room_Bedroom>() orderby room.transform.position.x ascending select room).ToArray();

        for (int i = 0; i < bedrooms.Length; i++)
        {
            //old instantiation code
            //bedrooms[i] = Instantiate(Resources.Load<GameObject>(BedroomPrefabPath), BedroomPosition.position + (Vector3.right * i * 5), BedroomPosition.rotation, transform).GetComponent<Room_Bedroom>();

            bedrooms[i].SelfInitialize(RoomData.CoveredIndizes[Mathf.CeilToInt(i * 2) + 1].GetBack().GetLeft(), this);

        }

        #region Manual assignment of grid index movement directions
        for (int i = 0; i < RoomData.CoveredIndizes.Length; i++)
        {

            GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[i], Enums.MoveDirections.Back);

            if (i % 2 == 1)
            {
                GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[i], Enums.MoveDirections.D_BackLeft);
            }
            else
            {
                GridManager.Ref.AddMovementDirectionToGridIndex(RoomData.CoveredIndizes[i], Enums.MoveDirections.D_BackRight);
            }
            //GridManager.Ref.DebugIndexMovement(RoomData.CoveredIndizes[i], " : index " + i);
        }
        #endregion
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

    public Room_Bedroom GetBedroomFromRayCheck()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, float.PositiveInfinity, LayerMask.GetMask("Room"));
        return GetBedroomFromX(hitInfo.point.x - hitInfo.transform.position.x);
    }


    
    private bool CheckManPositionToBedrooms(ManScript man, out int index)
    {
        GridIndex pos = GridManager.Ref.GetXYGridIndexFromWorldPosition(man.transform.position);
        for (int i = 0; i < bedrooms.Length; i++)
        {
            if (pos == bedrooms[i].RoomData.CoveredIndizes[bedrooms[i].RoomData.CoveredIndizes.Length - 1])
            {
                index = i;
                return true;
            }
        }
        index = 0;
        return false;
    }
}
