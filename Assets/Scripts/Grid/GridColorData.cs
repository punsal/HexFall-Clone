using System;
using UnityEngine;

namespace Grid
{
    [Serializable]
    public struct GridColorData
    {
        public Color color;
        public GridColorType colorType;
        public int count;
        public int counter;

        public void ResetCounter() => counter = 0;
    }
}