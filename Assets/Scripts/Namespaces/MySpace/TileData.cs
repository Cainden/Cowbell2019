// Data container class for a cell on the room grid.

using System;

namespace MySpace
{
    public class TileData
    {
        private bool _Occupied;   // A room has been built here already
        private Guid _RoomId;
        private Enums.RoomSizes _RoomSize;

        public bool Occupied
        {
            get { return _Occupied; }
            set { _Occupied = value; }
        }

        public Guid RoomId
        {
            get { return _RoomId; }
            set { _RoomId = value; }
        }

        public Enums.RoomSizes RoomSize
        {
            get { return _RoomSize; }
            set { _RoomSize = value; }
        }


        public TileData()
        {

        }

        public TileData(bool occupied, Guid roomId, Enums.RoomSizes roomSize)
        {
            _Occupied = occupied; // A room has been built here already
            _RoomId = roomId;
            _RoomSize = roomSize;
        }
    }
}
