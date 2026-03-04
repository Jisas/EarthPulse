using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace UltimateFramework.TempSyncSystem
{
    [Serializable]
    public class InputPattern
    {
        public string name;     
        [Range(0, 10)] public float scalingPercentage;
        public List<InputStruct> pattern;
    }

    [Serializable]
    public struct InputStruct
    {
        public InputActionReference input;
        public bool requiresCollision;
    }
}
