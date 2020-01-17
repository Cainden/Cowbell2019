// Data container class for a man TYPE definition.

using System;
using UnityEngine;

namespace MySpace
{
    public class ManDefData
    {
        private string _ManName;
        private string _ManModelFile;
        private Enums.ManTypes _ManType;

        public string ManName
        {
            get { return _ManName; }
            set { _ManName = value; }
        }

        public string ManModelFile
        {
            get { return _ManModelFile; }
            set { _ManModelFile = value; }
        }

        public Enums.ManTypes ManType
        {
            get { return _ManType; }
            set { _ManType = value; }
        }

        public ManDefData(string manName, string manModelFile, Enums.ManTypes manType)
        {
            _ManName = manName;
            _ManModelFile = manModelFile;
            _ManType = manType;
        }
    }
}