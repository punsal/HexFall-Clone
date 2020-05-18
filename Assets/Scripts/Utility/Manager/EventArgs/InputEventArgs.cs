using UnityEngine;
using Utility.Manager.Type;

namespace Utility.Manager.EventArgs
{
    public struct InputEventArgs
    {
        public InputState InputState;
        public Vector3 CurrentWorldPosition;
        public Vector3 Delta;
        public float magnitude;
    }
}