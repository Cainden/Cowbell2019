// Container class for avatar/man references (used by ManManager)

using System;
using UnityEngine;

namespace MySpace
{
    // Simple dataset to reference Man objects. All other data is stored in object script
    public struct ManRef<T> where T : ManScript
    {
        public GameObject ManObject { get; set; }

        public T ManScript { get; set; }

        public ManRef(GameObject manObject, T manScript)
        {
            ManObject = manObject;
            ManScript = manScript;
        }
    }
}
