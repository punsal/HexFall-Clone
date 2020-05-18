using System;
using UnityEngine;

namespace Utility.UI.Layout.Grid.Data
{
    [Serializable]
    public struct CellSizeData
    {
        public Vector2 aspectRatio;
        public Vector2 cellSize;

        public float Calculate()
        {
            return aspectRatio.x / aspectRatio.y;
        }
    }
}