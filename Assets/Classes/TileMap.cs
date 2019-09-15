// Container for movement directions of the tile grid

using System;
using UnityEngine;

namespace MySpace
{
    public class TileMap
    {
        private byte[,,] _GridMovements;

        public TileMap()
        {
            _GridMovements = new byte[Constants.GridSizeX, Constants.GridSizeY, Constants.GridSizeZ];
        }

        public TileMap(TileMap otherMap)
        {
            if (otherMap == null) throw new ArgumentNullException("otherMap");
            _GridMovements = (byte[,,])otherMap._GridMovements.Clone();
        }

        public void ClearMovements(GridIndex[] indizes)
        {
            if (indizes == null) throw new ArgumentNullException("indizes");
            foreach (GridIndex Index in indizes)
            {
                ClearMovements(Index);
            }
        }

        public void ClearMovements(GridIndex index)
        {
            if (index == null) return;

            // Also considering linked surrounding tiles. Just removing all possible
            RemoveMovementDirection(index.GetBelow(), Enums.MoveDirections.Top);
            RemoveMovementDirection(index.GetAbove(), Enums.MoveDirections.Bottom);
            RemoveMovementDirection(index.GetLeft(), Enums.MoveDirections.Right);
            RemoveMovementDirection(index.GetRight(), Enums.MoveDirections.Left);
            RemoveMovementDirection(index.GetFront(), Enums.MoveDirections.Back);
            RemoveMovementDirection(index.GetBack(), Enums.MoveDirections.Front);
            _GridMovements[index.X, index.Y, index.Z] = 0;
        }

        public void LinkTiles(GridIndex index1, GridIndex index2, Enums.MoveDirections dir)
        {
            AddMovementDirection(index2, GetOppositeMovementDirection(dir));
            AddMovementDirection(index1, dir);
        }

        public static Enums.MoveDirections GetOppositeMovementDirection(Enums.MoveDirections dir)
        {
            switch (dir)
            {
                case Enums.MoveDirections.Top: return (Enums.MoveDirections.Bottom);
                case Enums.MoveDirections.Bottom: return (Enums.MoveDirections.Top);
                case Enums.MoveDirections.Left: return (Enums.MoveDirections.Right);
                case Enums.MoveDirections.Right: return (Enums.MoveDirections.Left);
                case Enums.MoveDirections.Front: return (Enums.MoveDirections.Back);
                default: return (Enums.MoveDirections.Front); // Submitted 'Back', obviously
            }
        }

        void AddMovementDirection(GridIndex Index, Enums.MoveDirections dir)
        {
            _GridMovements[Index.X, Index.Y, Index.Z] |= (byte)dir;
        }

        void RemoveMovementDirection(GridIndex Index, Enums.MoveDirections dir)
        {
            if (!Index.IsValid()) return;
            _GridMovements[Index.X, Index.Y, Index.Z] &= (byte)~dir;
        }

        public int CountLinksFromGridTile(GridIndex index)
        {
            int Count = 0;
            foreach (Enums.MoveDirections Dir in Enum.GetValues(typeof(Enums.MoveDirections)))
            {
                if (GridTileHasDirection(index, Dir)) Count++;
            }
            return (Count);
        }

        public bool GridTileHasDirection(GridIndex index, Enums.MoveDirections dir)
        {
            if (index == null) return (false);
            if ((_GridMovements[index.X, index.Y, index.Z] & (byte)dir) != 0) return (true);
            return (false);
        }

        public void DEBUG_DumpGrid()
        {
            string Line = "";

            for (int z = 0; z < Constants.GridSizeZ; z++)
            {
                for (int y = Constants.GridSizeY - 1; y >= 0; y--)
                {
                    for (int x = 0; x < Constants.GridSizeX; x++)
                    {
                        Line = Line + _GridMovements[x, y, z].ToString("d2") + " | ";
                    }
                    Line += "\r\n";
                }
                Line += "\r\n";
            }

            Debug.Log(Line);
        }
    }
}
