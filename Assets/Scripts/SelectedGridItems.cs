using System;
using Grid.Position;
using Item;
using Item.Neighbor.Direction;
using UnityEngine;

[Serializable]
public struct SelectedGridItems
{
    public GridPosition gridPosition;
    public Vector3 inputPosition;
    public GridItem root;
    public GridItem neighbor1;
    public NeighborDirection neighbor1Direction;
    public GridItem neighbor2;
    public NeighborDirection neighbor2Direction;
}