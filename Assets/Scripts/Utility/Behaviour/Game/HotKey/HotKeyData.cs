using System;
using EventArguments;
using UnityEngine;

namespace Utility.Behaviour.Game.HotKey
{
    [Serializable]
    public struct HotKeyData
    {
        public KeyCode keyCode;
        public GameEventType eventType;
    }
}