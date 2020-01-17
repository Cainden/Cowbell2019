// Container class for room references (used by RoomManager)

using System;
using UnityEngine;

namespace MySpace
{
    // Simple dataset to reference Room objects. All other data is stored in object script
    public class RoomRef
    {
        private GameObject _RoomObject;
        private RoomScript _RoomScript;

        public GameObject RoomObject
        {
            get { return _RoomObject; }
            set { _RoomObject = value; }
        }

        public RoomScript RoomScript
        {
            get { return _RoomScript; }
            set { _RoomScript = value; }
        }

        public RoomRef(GameObject roomObject, RoomScript roomScript)
        {
            _RoomObject = roomObject;
            _RoomScript = roomScript;
        }
    }
}
