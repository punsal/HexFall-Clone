using System;
using UnityEngine.Events;
using Utility.System.State_Machine_System.Type;

namespace Utility.System.State_Machine_System.Data
{
    [Serializable]
    public struct State
    {
        public GameState currentGameState;
        public GameState nextGameState;

        public UnityEvent onStateEvent;
    }
}