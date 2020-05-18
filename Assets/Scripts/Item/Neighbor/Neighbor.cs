using System;
using Item.Neighbor.Direction;

namespace Item.Neighbor
{
    [Serializable]
    public struct Neighbor
    {
        public NeighborDirection direction;
        public GridItem item;
    }
}