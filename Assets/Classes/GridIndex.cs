// Helper Class for managing a single index on the 3D room grid.

using System;

namespace MySpace
{
    [Serializable]
    public class GridIndex
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public GridIndex()
        {

        }

        public GridIndex(GridIndex other)
        {
            if (other == null) throw new ArgumentNullException("other");
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public GridIndex(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public GridIndex Add(GridIndex other)
        {
            return (this + other);
        }

        public static GridIndex operator +(GridIndex c1, GridIndex c2)
        {
            if (c1 == null) throw new ArgumentNullException("c1");
            if (c2 == null) throw new ArgumentNullException("c2");

            GridIndex r = new GridIndex();
            r.X = c1.X + c2.X;
            r.Y = c1.Y + c2.Y;
            r.Z = c1.Z + c2.Z;
            return (r);
        }

        public GridIndex Subtract(GridIndex other)
        {
            return (this - other);
        }

        public static GridIndex operator -(GridIndex c1, GridIndex c2)
        {
            if (c1 == null) throw new ArgumentNullException("c1");
            if (c2 == null) throw new ArgumentNullException("c2");

            GridIndex r = new GridIndex();
            r.X = c1.X - c2.X;
            r.Y = c1.Y - c2.Y;
            r.Z = c1.Z - c2.Z;
            return (r);
        }

        public static bool operator ==(GridIndex c1, GridIndex c2)
        {
            if (ReferenceEquals(c1, c2)) return (true);
            if (((object)c1 == null) || ((object)c2 == null)) return (false);
            return (c1.X == c2.X && c1.Y == c2.Y && c1.Z == c2.Z);
        }

        public static bool operator !=(GridIndex c1, GridIndex c2)
        {
            return !(c1 == c2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return (false);
            GridIndex p = obj as GridIndex;
            if ((object)p == null) return (false);
            return (this == p);
        }

        public override int GetHashCode()
        {
            return (X + Y * Constants.GridSizeX + Z * Constants.GridSizeX * Constants.GridSizeY); // Unique number
        }

        public void SetXY(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool IsValid()
        {
            if ((X < 0) || (Y < 0) || (Z < 0)) return (false);
            if ((X >= Constants.GridSizeX) || (Y >= Constants.GridSizeY) || (Z >= Constants.GridSizeZ)) return (false);
            return (true);
        }

        public static bool IsValid(GridIndex[] indexArray)
        {
            if (indexArray == null) return (false);
            if (indexArray.Length == 0) return (false);

            for (int i = 0; i < indexArray.Length; i++)
            {
                if (indexArray[i].IsValid() == false) return (false);
            }

            return (true);
        }

        public GridIndex GetLeft()
        {
            GridIndex l = new GridIndex(-1, 0, 0) + this;
            return (l);
        }

        public GridIndex[] GetLeft(int count)
        {
            GridIndex[] indexArray = new GridIndex[count];
            for (int i = 0; i < count; i++) indexArray[i] = new GridIndex(-1 - i, 0, 0) + this;
            Array.Reverse(indexArray); // Reverse, so we always count from left to right
            return (indexArray);
        }

        public GridIndex GetRight()
        {
            GridIndex r = new GridIndex(1, 0, 0) + this;
            return (r);
        }

        public GridIndex[] GetRight(int count)
        {
            GridIndex[] indexArray = new GridIndex[count];
            for (int i = 0; i < count; i++) indexArray[i] = new GridIndex(1 + i, 0, 0) + this;
            return (indexArray);
        }

        public GridIndex GetAbove()
        {
            GridIndex a = new GridIndex(0, 1, 0) + this;
            return (a);
        }

        public GridIndex GetBelow()
        {
            GridIndex b = new GridIndex(0, -1, 0) + this;
            return (b);
        }

        public GridIndex GetFront()
        {
            GridIndex a = new GridIndex(0, 0, -1) + this;
            return (a);
        }

        public GridIndex GetBack()
        {
            GridIndex b = new GridIndex(0, 0, 1) + this;
            return (b);
        }

        public bool IsFrontPlane()
        {
            return (Z == 0);
        }

        public bool IsBackPlane()
        {
            return (Z == 1);
        }

        public bool IsAdjacentTo(GridIndex otherIndex)
        {
            if (otherIndex == null) return (false);

            if (Math.Abs(X - otherIndex.X) > 1) return (false);
            if (Math.Abs(Y - otherIndex.Y) > 1) return (false);
            if (Math.Abs(Z - otherIndex.Z) > 1) return (false);
            return (true);
        }

        public GridIndex GetAdjacent(Enums.MoveDirections dir)
        {
            switch (dir)
            {
                case Enums.MoveDirections.Bottom: return (GetBelow());
                case Enums.MoveDirections.Top: return (GetAbove());
                case Enums.MoveDirections.Left: return (GetLeft());
                case Enums.MoveDirections.Right: return (GetRight());
                case Enums.MoveDirections.Front: return (GetFront());
                case Enums.MoveDirections.Back: return (GetBack());
                default: return (this);
            }
        }

        public bool OtherIndexIsInDirection(GridIndex otherIndex, Enums.MoveDirections dir)
        {
            if (otherIndex == null) return (false);

            switch (dir)
            {
                case Enums.MoveDirections.Top:
                    if ((otherIndex.Y - Y) > 0) return (true); break;
                case Enums.MoveDirections.Bottom:
                    if ((Y - otherIndex.Y) > 0) return (true); break;
                case Enums.MoveDirections.Left:
                    if ((X - otherIndex.X) > 0) return (true); break;
                case Enums.MoveDirections.Right:
                    if ((otherIndex.X - X) > 0) return (true); break;
                case Enums.MoveDirections.Front:
                    if ((Z - otherIndex.Z) > 0) return (true); break;
                case Enums.MoveDirections.Back:
                    if ((otherIndex.Z - Z) > 0) return (true); break;
            }

            return (false);
        }

        public override string ToString()
        {
            return (X.ToString() + "/" + Y.ToString() + "/" + Z.ToString());
        }
    }
}