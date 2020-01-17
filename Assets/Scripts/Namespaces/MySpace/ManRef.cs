// Container class for avatar/man references (used by ManManager)

using System;
using UnityEngine;

namespace MySpace
{
    // Simple dataset to reference Man objects. All other data is stored in object script
    public class ManRef
    {
        private GameObject _ManObject;
        private ManScript _ManScript;

        public GameObject ManObject
        {
            get { return _ManObject; }
            set { _ManObject = value; }
        }

        public ManScript ManScript
        {
            get { return _ManScript; }
            set { _ManScript = value; }
        }

        public ManRef(GameObject manObject, ManScript manScript)
        {
            ManObject = manObject;
            ManScript = manScript;
        }
    }
}
