// This one is dealing with building new rooms

using MySpace;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [HideInInspector]
    public static BuildManager Ref { get; private set; } // For external access of script
 
    private List<GameObject> RoomPositionSelectors = new List<GameObject>();

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<BuildManager>();
    }

    public void ShowRoomPositionSelectors(GridIndex[] indexArray, Enums.RoomTypes roomType, Enums.RoomSizes roomSize, Enums.RoomOverUnder overUnder)
    {
        if (indexArray == null) return;

        if (indexArray.Length == 0)
        {
            StateManager.Ref.SetGameState(Enums.GameStates.Normal);
            GuiManager.Ref.Initiate_UserInfoSmall("Sorry, can't build a room of this type now!");
            return;
        }

        for (int i = 0; i < indexArray.Length; i++)
        {
            Debug.Assert(roomSize != Enums.RoomSizes.None);

            //CHECK TO SEE IF BUILDING IS UNDER/OVERWORLD, SKIP TO NEXT ITERATION IF POSITION IS NOT COMPATIBLE WITH OVERUNDER TYPE
            int yPos = indexArray[i].Y;

            if ((yPos < Constants.GridSurfaceY && (int)overUnder == 1) ||
                (yPos >= Constants.GridSurfaceY && (int)overUnder == -1))
                continue;

            GameObject go = Instantiate(Resources.Load<GameObject>(Constants.RoomBuildSelectorModels[roomSize]));
            Debug.Assert(go != null);

            go.transform.position = GridManager.Ref.GetWorldPositionFromGridIndex(indexArray[i]);

            go.transform.GetComponent<BuildPositionScript>().SetRoomInfoData(roomSize, roomType, overUnder, indexArray[i]);
            go.SetActive(true);

            RoomPositionSelectors.Add(go);
        }
    }

    public void HideRoomPositionSelectors()
    {
        RoomPositionSelectors.ForEach(item => Destroy(item));
        RoomPositionSelectors.Clear();
    }

    public void Build_Finished(Enums.RoomSizes roomSize, Enums.RoomTypes roomType, Enums.RoomOverUnder roomOverUnder, GridIndex index)
    {
        HideRoomPositionSelectors();
        RoomManager.Ref.CreateRoom(Guid.NewGuid(), roomType, roomSize, roomOverUnder, index);        
    }
}
