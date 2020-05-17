// Managing the buidling grid. E.g., storing which room is covering which index.

using MySpace;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [HideInInspector]
    public static GridManager Ref { get; private set; } // For external access of script

    // Here, common info is stored. E.g., what room covers the tile, Also covering Z for pathfinding with Elevators
    private static TileData[,,] _GridData = new TileData[Constants.GridSizeX, Constants.GridSizeY, Constants.GridSizeZ];

    // Here, links between the grid tiles are stored. Kept separate for use in pathfinding
    private static TileMap _GridMovements = new TileMap();

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<GridManager>();
        InitGridData();
    }

    private void InitGridData()
    {
        for (int x = 0; x < Constants.GridSizeX; x++)
            for (int y = 0; y < Constants.GridSizeY; y++)
                for (int z = 0; z < Constants.GridSizeZ; z++)
                    _GridData[x, y, z] = new TileData(false, Guid.Empty, Enums.RoomSizes.None);
                    // No need to initialize _GridMovements here
    }

    public GridIndex[] GetOccupiedindizes(Enums.RoomSizes roomSize, GridIndex leftMostIndex)
    {
        if (leftMostIndex == null) throw new ArgumentNullException("leftMostIndex");

        GridIndex[] Occupiedindizes;
        
        if (roomSize == Enums.RoomSizes.Size1)
            Occupiedindizes = new GridIndex[] { new GridIndex(leftMostIndex), leftMostIndex.GetBack() };
        else
            Occupiedindizes = leftMostIndex.GetLeft().GetRight((int)roomSize);

        return (Occupiedindizes);
    }

    // When a new room is created, set grid indizes and internal links accordingly
    public void RegisterAtGrid(Enums.RoomSizes roomSize, Guid roomId, GridIndex leftMostIndex, bool linkRoom = true)
    {
        GridIndex[] Occupiedindizes = GetOccupiedindizes(roomSize, leftMostIndex);

        for (int i = 0; i < Occupiedindizes.Length; i++)
        {
            _GridData[Occupiedindizes[i].X, Occupiedindizes[i].Y, Occupiedindizes[i].Z] = new TileData(true, roomId, roomSize);
        }

        if (linkRoom)
            LinkRoom(Occupiedindizes); // Calculate and store movements to other grid elements
    }    
    
    public bool IsGridAreaFree(GridIndex leftMostIndex, Enums.RoomSizes roomSize)
    {
        GridIndex[] Occupiedindizes = GetOccupiedindizes(roomSize, leftMostIndex);
        return (IsGridAreaFree(Occupiedindizes));
    }

    public bool IsGridAreaFree(GridIndex[] indizes)
    {
        if (indizes == null) throw new ArgumentNullException("indizes");

        for (int i = 0; i < indizes.Length; i++)
        {
            if (_GridData[indizes[i].X, indizes[i].Y, indizes[i].Z].Occupied == true) return (false);
        }
        return (true);
    }

    // When a room is removed, set grid indizes accordingly
    public void DeregisterFromGrid(GridIndex leftMostIndex, Enums.RoomSizes roomSize)
    {
        GridIndex[] Occupiedindizes = GetOccupiedindizes(roomSize, leftMostIndex);
        
        for (int i = 0; i < Occupiedindizes.Length; i++)
        {
            _GridData[Occupiedindizes[i].X, Occupiedindizes[i].Y, Occupiedindizes[i].Z] = new TileData();
        }

        UnlinkRoom(Occupiedindizes); // Remove movements to other grid elements
    }

    public Vector3 GetWorldPositionFromGridIndex(GridIndex index)
    {
        if (index == null) throw new ArgumentNullException("index");

        Vector3 vPos = new Vector3(index.X * Constants.GridElementWidth,
                                   index.Y * Constants.GridElementHeight,
                                   index.Z * Constants.GridElementDepth);
        
        // Applying the offset between underground and above
        if (index.Y >= Constants.GridSurfaceY) vPos.y += (Constants.GridElementHeight / 2.0f);

        return (vPos);
    }

    public GridIndex GetXYGridIndexFromWorldPosition(Vector3 worldPos)
    {
        if ((worldPos.x < 0.0f) || (worldPos.y < 0.0f)) return (new GridIndex()); // Return invalid index
        if ((worldPos.x > (Constants.GridElementWidth * Constants.GridSizeX)) ||
            (worldPos.y > (Constants.GridElementHeight * Constants.GridSizeY + (Constants.GridElementHeight / 2.0f))))
            return (new GridIndex());

        int IndexX = (int) Math.Floor(worldPos.x / Constants.GridElementWidth);

        // Checking for the offset between underground and above
        if (worldPos.y > (Constants.GridSurfaceY * Constants.GridElementHeight)) worldPos.y -= (Constants.GridElementHeight / 2.0f);

        int IndexY = (int)Math.Floor(worldPos.y / Constants.GridElementHeight);

        return (new GridIndex(IndexX, IndexY, 0));
    }

    public Vector3 GetWorldPositionFromGridIndexZOffset(GridIndex index, float zOffset)
    {
        Vector3 vPos = GetWorldPositionFromGridIndex(index);
        vPos.z += zOffset;
        return (vPos);
    }
    
    public Enums.RoomSizes GetGridTileRoomSize(GridIndex index)
    {
        if (index == null) throw new ArgumentNullException("index");
        Debug.Assert(index.IsValid() == true);
        return (_GridData[index.X, index.Y, index.Z].RoomSize);
    }

    public Guid GetGridTileRoomGuid(GridIndex index)
    {
        if (index == null) throw new ArgumentNullException("index");
        //Debug.Assert(index.IsValid() == true);
        if (!index.IsValid())
            return Guid.Empty;
        return (_GridData[index.X, index.Y, index.Z].RoomId);
    }

    //Note: Made this a struct and went pretty modular in case we wanted to specify any other building restrictions later down the line. Might need to add roomtype into the restrictions if that's the case.
    #region Build Highlighting
    internal enum Bool : byte { True = 1, False = 2, Null = 0 }

    public struct BuildInfo
    {
        public GridIndex index;
        public bool isDouble, goDown;

        public BuildInfo(GridIndex index)
        {
            this.index = index;
            isDouble = goDown = false;
        }

        public BuildInfo(GridIndex index, bool doubl)
        {
            this.index = index;
            isDouble = doubl;
            goDown = false;
        }

        public BuildInfo(GridIndex index, bool doubl, bool goDown)
        {
            this.index = index;
            isDouble = doubl;
            this.goDown = goDown;
        }

        public static bool ListContainsIndex(List<BuildInfo> list, GridIndex index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == index)
                    return true;
            }
            return false;
        }

        internal static Bool ListIndexIsDouble(List<BuildInfo>list, GridIndex index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == index)
                {
                    if (list[i].isDouble)
                        return Bool.True;
                    else
                        return Bool.False;
                }
            }
            return Bool.Null;
        }

        internal static Bool ListIndexIsDouble(List<BuildInfo> list, GridIndex index, out bool down)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == index)
                {
                    down = list[i].goDown;
                    if (list[i].isDouble)
                        return Bool.True;
                    else
                        return Bool.False;
                }
            }
            down = false;
            return Bool.Null;
        }

        public static void RemoveIndexFromBuildList(List<BuildInfo> list, GridIndex index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == index)
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        #region Operators & Overrides

        public static bool operator ==(BuildInfo a, GridIndex b)
        {
            return a.index == b;
        }

        public static bool operator !=(BuildInfo a, GridIndex b)
        {
            return a.index != b;
        }

        public static bool operator ==(BuildInfo a, BuildInfo b)
        {
            return a == b;
        }

        public static bool operator !=(BuildInfo a, BuildInfo b)
        {
            return a != b;
        }

        public static bool operator ==(GridIndex b, BuildInfo a)
        {
            return a.index == b;
        }

        public static bool operator !=(GridIndex b, BuildInfo a)
        {
            return a.index != b;
        }

        //To remove annoying green errors
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    public BuildInfo[] GetPossibleBuildingindizes(Enums.RoomSizes roomSize)
    {
        // Note: Returning only the leftmost index per possible location, as this is reference for building

        // Building Rules:
        // We can build rooms of size 1/2/4/6 right and left of of existing rooms
        // We can build room size 1 below and above existing size 1 rooms
        
        List<BuildInfo> indexList = new List<BuildInfo>();
        CheckBuildingPositionsInRange(0, Constants.GridSizeY, roomSize, ref indexList);

        return (indexList.ToArray()); // Can also be empty
    }

    void CheckBuildingPositionsInRange(int y0, int y1, Enums.RoomSizes roomSize, ref List<BuildInfo> indexList)
    {
        // Only checking the front plane for this (Z = 0)
        for (int y = y0; y < y1; y++)
            for (int x = 0; x < Constants.GridSizeX; x++)
            {
                GridIndex index = new GridIndex(x, y, 0); // Grid index we now look at
                if (_GridData[index.X, index.Y, 0].Occupied == false) continue; // Nothing there, so can not build next to it            

                // Special for size1 room: We can build above/below a size 1 because it is an elevator
                if ((roomSize == _GridData[x, y, 0].RoomSize) && (roomSize == Enums.RoomSizes.Size1))
                {
                    CheckAboveBelowBuildingPositions(index, ref indexList);
                }

                CheckLeftRightBuildingPositions(index, roomSize, ref indexList);
            }
    }

    void CheckAboveBelowBuildingPositions(GridIndex index, ref List<BuildInfo> indexList)
    {
        if (index == null) throw new ArgumentNullException("index");
        if (indexList == null) throw new ArgumentNullException("indexList");

        // Only checking the front plane for this (Z = 0)
        if (index.Y != Constants.GridSurfaceY - 1)
        {
            GridIndex IndexAbove = index.GetAbove();
            if (IndexAbove.IsValid() && (_GridData[IndexAbove.X, IndexAbove.Y, 0].Occupied == false))
            {
                if (!BuildInfo.ListContainsIndex(indexList, IndexAbove))
                {
                    DoubleCheckBuildingPositionsSize1(IndexAbove, ref indexList);
                }
                    
            }
        }

        if (index.Y != Constants.GridSurfaceY)
        {
            GridIndex IndexBelow = index.GetBelow();
            if (IndexBelow.IsValid() && (_GridData[IndexBelow.X, IndexBelow.Y, 0].Occupied == false))
            {
                if (!BuildInfo.ListContainsIndex(indexList, IndexBelow))
                {
                    DoubleCheckBuildingPositionsSize1(IndexBelow, ref indexList);
                }
                    
            }
        }
    }

    void CheckLeftRightBuildingPositions(GridIndex index, Enums.RoomSizes roomSize, ref List<BuildInfo> indexList)
    {
        // Left check
        GridIndex[] indizes = index.GetLeft((int)roomSize);
        if (GridIndex.IsValid(indizes) && IsGridAreaFree(indizes))
        {
            if (!BuildInfo.ListContainsIndex(indexList, indizes[0]))
            {
                if (roomSize == Enums.RoomSizes.Size1)
                {
                    DoubleCheckBuildingPositionsSize1(indizes[0], ref indexList);
                }
                else
                    indexList.Add(new BuildInfo(indizes[0]));
            }
                
        }

        // Right check
        indizes = index.GetRight((int)roomSize);
        if (GridIndex.IsValid(indizes) && IsGridAreaFree(indizes))
        {
            if (!BuildInfo.ListContainsIndex(indexList, indizes[0]))
            {
                if (roomSize == Enums.RoomSizes.Size1)
                {
                    DoubleCheckBuildingPositionsSize1(indizes[0], ref indexList);
                }
                else 
                    indexList.Add(new BuildInfo(indizes[0]));
            }

        }
    }

    void DoubleCheckBuildingPositionsSize1(GridIndex index, ref List<BuildInfo> indexList)
    {
        GridIndex checkB = index.GetBelow(), checkA = index.GetAbove(), checkBB = checkB.GetBelow(), checkAA = checkA.GetAbove();
        bool botGood = checkB.IsValid() && (_GridData[checkB.X, checkB.Y, 0].Occupied == false) && checkB.Y > Constants.GridSurfaceY;
        bool topGood = checkA.IsValid() && (_GridData[checkA.X, checkA.Y, 0].Occupied == false) && index.Y >= Constants.GridSurfaceY;

        if (RoomManager.Ref.GetRoomData(checkB)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator 
            || RoomManager.Ref.GetRoomData(checkA)?.RoomScript.RoomData.RoomType == Enums.RoomTypes.Elevator)
        {
            if (!BuildInfo.ListContainsIndex(indexList, index))
            {
                indexList.Add(new BuildInfo(index));
            }
            return;
        }

        if (botGood)
        {
            //will return null only when the list does not contain the index, otherwise it returns true or false depending on the isDouble value of the buildinfo index
            //d is whether or not the doublehighlight is going downward
            Bool has = BuildInfo.ListIndexIsDouble(indexList, checkBB, out bool d);
            Bool has1 = BuildInfo.ListIndexIsDouble(indexList, checkB, out bool d1);
            switch (has)
            {
                case Bool.True:
                    //Even if the list contains two below, we can build going down here because that one is also going down
                    if (d && has1 == Bool.Null)
                    {
                        indexList.Add(new BuildInfo(index, true, true));
                    }
                    //else
                    //Do nothing here, the list already contains this index with a double going upward
                    break;

                case Bool.False:
                    if (has1 == Bool.Null)
                    {
                        indexList.Add(new BuildInfo(checkB, true, true));
                        BuildInfo.RemoveIndexFromBuildList(indexList, checkB); //If there was a position added with a double available to be built from another adjacent spot, wouldnt want to give the player a feelsbad
                    }
                    return;

                case Bool.Null:
                    //returned null, so the list does not contain this index yet.
                    if (has1 == Bool.Null)
                        indexList.Add(new BuildInfo(index, true, true));
                    return;
            }
            return;
        }
        
        if (topGood)
        {
            Bool h = BuildInfo.ListIndexIsDouble(indexList, checkB, out bool d);
            switch (h)
            {
                case Bool.True:
                    if (d)
                    {
                        Bool has1 = BuildInfo.ListIndexIsDouble(indexList, checkA, out bool d1);
                        if (has1 == Bool.Null)
                        {
                            indexList.Add(new BuildInfo(checkA, true, true));
                        }
                        //indexList.Add(new BuildInfo(index, true));
                    }
                    //else
                    //Do nothing here, the list already contains this index within a double going upward
                    break;

                case Bool.False:
                    //Do nothing here, the list already contains this index
                    break;

                case Bool.Null:
                    //returned null, so the list does not contain this index yet.
                    indexList.Add(new BuildInfo(index, true));
                    break;
            }

        }
        
    }

    #endregion

    private void LinkRoom(GridIndex[] occupiedIndizes)
    {
        // Step through all indizes and link (internally and to others)
        // On front plane, we can only link to left/right/back
        // On back plane, we can only link to above/below/front

        Enums.MoveDirections[] FrontPlaneDirections = { Enums.MoveDirections.Left, Enums.MoveDirections.Right, Enums.MoveDirections.Back };
        Enums.MoveDirections[] BackPlaneDirections = { Enums.MoveDirections.Top, Enums.MoveDirections.Bottom, Enums.MoveDirections.Front };

        for (int i = 0; i < occupiedIndizes.Length; i++)
        {
            if (occupiedIndizes[i].IsFrontPlane())
            {
                foreach (Enums.MoveDirections Dir in FrontPlaneDirections)
                {
                    GridIndex AdjIndex = occupiedIndizes[i].GetAdjacent(Dir);
                    if (!AdjIndex.IsValid()) continue;
                    if (!_GridData[AdjIndex.X, AdjIndex.Y, AdjIndex.Z].Occupied) continue;
                    _GridMovements.LinkTiles(occupiedIndizes[i], AdjIndex, Dir);
                }
            }
            else // Is backplane
            {
                foreach (Enums.MoveDirections Dir in BackPlaneDirections)
                {
                    GridIndex AdjIndex = occupiedIndizes[i].GetAdjacent(Dir);
                    if (!AdjIndex.IsValid()) continue;
                    if (!_GridData[AdjIndex.X, AdjIndex.Y, AdjIndex.Z].Occupied) continue;



                    // Unless the rooms are entrance rooms...
                    // If the current room is on the surface, don't link to a bottom room
                    // If the current room is one below the surface, don't link to a top room

                    if (((occupiedIndizes[i].Y == Constants.GridSurfaceY && Dir == Enums.MoveDirections.Bottom) ||
                        (occupiedIndizes[i].Y == Constants.GridSurfaceY - 1 && Dir == Enums.MoveDirections.Top)))
                    {

                        continue;
                    }
                    _GridMovements.LinkTiles(occupiedIndizes[i], AdjIndex, Dir);
                }
            }
        }
    }

    private void UnlinkRoom(GridIndex[] occupiedIndizes)
    {
        // Unlnk the occupied grid elements - also considering links to these tiles from others
        _GridMovements.ClearMovements(occupiedIndizes);
    }    
    
    public int CountLinksFromGridTile(GridIndex index)
    {
        return (_GridMovements.CountLinksFromGridTile(index));
    }

    public GridIndex[] GetIndexPath(GridIndex startIndex, GridIndex endIndex)
    {
        return (GetIndexPath(startIndex, endIndex, _GridMovements));
    }

    public GridIndex[] GetIndexPath(GridIndex startIndex, GridIndex endIndex, TileMap gridMovements)
    {
        List<GridIndex> IndexPath = PathFinder.FindPath(startIndex, endIndex, gridMovements);
        return (IndexPath.ToArray());
    }

    public Vector3[] GetPositionPath(GridIndex startIndex, GridIndex endIndex)
    {
        GridIndex[] IndexPath = GetIndexPath(startIndex, endIndex, _GridMovements);
        List<Vector3> PositionPathList = new List<Vector3>();

        for (int i = 0; i < IndexPath.Length; i++)
        {
            PositionPathList.Add(GetWorldPositionFromGridIndex(IndexPath[i]));
        }

        return (PositionPathList.ToArray());
    }

    public bool HasPathToEntrance(GridIndex startIndex, TileMap tileMap)
    {        
        GridIndex[] indexPath = GetIndexPath(startIndex, Constants.EntranceRoomIndex, tileMap);
        return (indexPath.Length != 0);
    }

    // Special procedure to check if access is still ok when a size1 room is to be removed
    public bool CanTilesBeRemovedSafely(GridIndex[] indizes)
    {
        // A temp movement map without the indizes we want to remove
        TileMap tmpMap = new TileMap(_GridMovements);
        tmpMap.ClearMovements(indizes);
        
        foreach (GridIndex Index in indizes)
        {
            foreach (Enums.MoveDirections Dir in Enum.GetValues(typeof(Enums.MoveDirections)))
            {
                if (_GridMovements.GridTileHasDirection(Index, Dir))
                {
                    GridIndex OtherIndex = Index.GetAdjacent(Dir);
                    if (Array.IndexOf(indizes, OtherIndex) > -1) continue;
                    if (HasPathToEntrance(OtherIndex, tmpMap) == false)
                    {
                         return (false);
                    }
                }
            }
        }
        return (true);
    }

    public void AddMovementDirectionToGridIndex(GridIndex index, Enums.MoveDirections dir)
    {
        _GridMovements.AddMovementDirection(index, dir);
    }

    public void RemoveMovementDirectionFromGridIndex(GridIndex index, Enums.MoveDirections dir)
    {
        _GridMovements.RemoveMovementDirection(index, dir);
    }

    public bool GridIndexHasDirection(GridIndex index, Enums.MoveDirections dir)
    {
        return _GridMovements.GridTileHasDirection(index, dir);
    }

    public void DebugIndexMovement(GridIndex index, string message)
    {
        _GridMovements.DebugGridIndexMovement(index, message);
    }

    public void EmptyGrid()
    {
        InitGridData();
        _GridMovements = new TileMap();
    }
}
