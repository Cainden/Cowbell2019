// This one is dealing with building new rooms

using MySpace;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [SerializeField] GameObject BuildSelector_Sz1, BuildSelector_Sz1_2, BuildSelector_Sz2, BuildSelector_Sz4, BuildSelector_Sz6;

    [HideInInspector]
    public static BuildManager Ref { get; private set; } // For external access of script
 
    private List<GameObject> RoomPositionSelectors = new List<GameObject>();

    public static event Action BuildFinishedEvent;

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<BuildManager>();
    }

    public void ShowRoomPositionSelectors(GridManager.BuildInfo[] indexArray, Enums.RoomTypes roomType, Enums.RoomSizes roomSize, Enums.RoomOverUnder overUnder)
    {
        if (indexArray == null) return;

        if (indexArray.Length == 0)
        {
            StateManager.Ref.SetGameState(Enums.GameStates.Normal);
            GuiManager.Ref.Initiate_UserInfoSmall("Sorry, can't build a room of this type now!");
            return;
        }
        Debug.Assert(roomSize != Enums.RoomSizes.None);

        for (int i = 0; i < indexArray.Length; i++)
        {
            //CHECK TO SEE IF BUILDING IS UNDER/OVERWORLD, SKIP TO NEXT ITERATION IF POSITION IS NOT COMPATIBLE WITH OVERUNDER TYPE
            int yPos = indexArray[i].index.Y;

            if ((yPos < Constants.GridSurfaceY && (int)overUnder == 1) || (yPos >= Constants.GridSurfaceY && (int)overUnder == -1))
            {
                continue;
            }
                

            GameObject go = Instantiate(GetPrefab(roomSize, indexArray[i].isDouble));
            Debug.Assert(go != null);

            if (indexArray[i].isDouble)
                go.transform.position = (Vector3.up * 0.2f) + (indexArray[i].goDown ? GridManager.Ref.GetWorldPositionFromGridIndex(indexArray[i].index.GetBelow()) : GridManager.Ref.GetWorldPositionFromGridIndex(indexArray[i].index));
            else
                go.transform.position = GridManager.Ref.GetWorldPositionFromGridIndex(indexArray[i].index);

            go.transform.GetComponent<BuildPositionScript>().SetRoomInfoData(roomSize, roomType, overUnder, indexArray[i].index);
            go.SetActive(true);

            RoomPositionSelectors.Add(go);
        }
    }

    public void HideRoomPositionSelectors()
    {
        RoomPositionSelectors.ForEach(item => Destroy(item));
        RoomPositionSelectors.Clear();
    }

    public void Build_Finished(Enums.RoomTypes roomType, Enums.RoomOverUnder roomOverUnder, GridIndex index)
    {
        HideRoomPositionSelectors();
        RoomManager.Ref.CreateRoom(Guid.NewGuid(), roomType, index);
        BuildFinishedEvent?.Invoke();
    }

    public GameObject GetPrefab(Enums.RoomSizes size, bool showDouble)
    {
        switch (size)
        {
            case Enums.RoomSizes.Size1:
                if (!showDouble)
                    return BuildSelector_Sz1;
                else
                    return BuildSelector_Sz1_2;
            case Enums.RoomSizes.Size2:
                return BuildSelector_Sz2;
            case Enums.RoomSizes.Size4:
                return BuildSelector_Sz4;
            case Enums.RoomSizes.Size6:
                return BuildSelector_Sz6;
            default:
                return null;
        }
    }
}
