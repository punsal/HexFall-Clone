using System;

namespace Grid.Position
{
    [Serializable]
    public struct GridPosition
    {
        public int rowPosition;
        public int columnPosition;

        public void SetPosition(int rowIndex, int columnIndex)
        {
            rowPosition = rowIndex;
            columnPosition = columnIndex;
        }
    }
}