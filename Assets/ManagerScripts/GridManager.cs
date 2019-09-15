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
    public void RegisterAtGrid(Enums.RoomSizes roomSize, Guid roomId, GridIndex leftMostIndex)
    {
        GridIndex[] Occupiedindizes = GetOccupiedindizes(roomSize, leftMostIndex);

        for (int i = 0; i < Occupiedindizes.Length; i++)
        {
            _GridData[Occupiedindizes[i].X, Occupiedindizes[i].Y, Occupiedindizes[i].Z] = new TileData(true, roomId, roomSize);
        }

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
        Debug.Assert(index.IsValid() == true);
        return (_GridData[index.X, index.Y, index.Z].RoomId);
    }

    public GridIndex[] GetPossibleBuildingindizes(Enums.RoomSizes roomSize)
    {
        // Note: Returning only the leftmost index per possible location, as this is reference for building

        // Building Rules:
        // We can build rooms of size 1/2/4/6 right and left of of existing rooms
        // We can build room size 1 below and above existing size 1 rooms
        
        List<GridIndex> indexList = new List<GridIndex>();
        CheckBuildingPositionsInRange(0, Constants.GridSizeY, roomSize, ref indexList);

        return (indexList.ToArray()); // Can also be empty
    }

    void CheckBuildingPositionsInRange(int y0, int y1, Enums.RoomSizes roomSize, ref List<GridIndex> indexList)
    {
        // Only checking the front plane for this (Z = 0)
        for (int y = y0; y < y1; y++)
            for (int x = 0; x < Constants.GridSizeX; x++)
            {
                GridIndex index = new GridIndex(x, y, 0); // Grid index we now look at
                if (_GridData[index.X, index.Y, 0].Occupied == false) continue; // Nothing there, so can not build next to it               

                // Special for size1 room: We can build above/below a size one
                if ((roomSize == _GridData[x, y, 0].RoomSize) && (roomSize == Enums.RoomSizes.Size1))
                {
                    CheckAboveBelowBuildingPositions(index, ref indexList);
                }

                CheckLeftRightBuildingPositions(index, roomSize, ref indexList);
            }
    }

    void CheckAboveBelowBuildingPositions(GridIndex index, ref List<GridIndex> indexList)
    {
        if (index == null) throw new ArgumentNullException("index");
        if (indexList == null) throw new ArgumentNullException("indexList");

        // Only checking the front plane for this (Z = 0)
        if (index.Y != Constants.GridSurfaceY - 1)
        {
            GridIndex IndexAbove = index.GetAbove();
            if (IndexAbove.IsValid() && (_GridData[IndexAbove.X, IndexAbove.Y, 0].Occupied == false))
            {
                if (indexList.Contains(IndexAbove) == false) indexList.Add(IndexAbove);
            }
        }

        if (index.Y != Constants.GridSurfaceY)
        {
            GridIndex IndexBelow = index.GetBelow();
            if (IndexBelow.IsValid() && (_GridData[IndexBelow.X, IndexBelow.Y, 0].Occupied == false))
            {
                if (indexList.Contains(IndexBelow) == false) indexList.Add(IndexBelow);
            }
        }
    }

    void CheckLeftRightBuildingPositions(GridIndex index, Enums.RoomSizes roomSize, ref List<GridIndex> indexList)
    {
        // Left check
        GridIndex[] indizes = index.GetLeft((int)roomSize);
        if (GridIndex.IsValid(indizes) && IsGridAreaFree(indizes))
        {
            if (indexList.Contains(indizes[0]) == false) indexList.Add(indizes[0]);
        }

        // Right check
        indizes = index.GetRight((int)roomSize);
        if (GridIndex.IsValid(indizes) && IsGridAreaFree(indizes))
        {
            if (indexList.Contains(indizes[0]) == false) indexList.Add(indizes[0]);
        }
    }

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

    public void EmptyGrid()
    {
        InitGridData();
        _GridMovements = new TileMap();
    }
}
