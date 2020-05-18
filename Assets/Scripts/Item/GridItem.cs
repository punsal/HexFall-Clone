using System;
using System.Collections.Generic;
using System.Linq;
using Cell_Particle;
using DG.Tweening;
using EventArguments;
using Grid;
using Grid.Position;
using Item.Neighbor.Direction;
using Item.Vertex;
using Item.Vertex.Direction;
using UnityEngine;
using UnityEngine.Events;
using Utility.Extension;
using Utility.System.Object_Pooler_System;
using Utility.System.Publisher_Subscriber_System;

namespace Item
{
    [ExecuteAlways]
    public class GridItem : MonoBehaviour
    {
        [Serializable]
        private struct PossibleMoveData
        {
            public Color color;
            public int count;

            public void AddCount(int addCount = 1) => count += addCount;
            public void RemoveCount(int removeCount = 1) => count -= removeCount;
        }
        
        [Header("Coloring")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color color;
        
        [Header("Controllers")]
        [SerializeField] private GridItemVertexController vertexController;

        [Header("Grid Position")]
        [SerializeField] private GridPosition position;

        [Header("Neighbors")]
        [SerializeField] private List<Neighbor.Neighbor> neighbors;

        [Header("Possible Move Data")]
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private int possibleMoveCount = 0;
        [SerializeField] private List<PossibleMoveData> possibleMoves;
        
        [Header("Events")]
        #pragma warning disable 649
        [SerializeField] private UnityEvent onSelect;
        [SerializeField] private UnityEvent onDeselect;
        #pragma warning restore 649

        public GridPosition Position
        {
            get => position;
            set => position = value;
        }
        
        private delegate void Selected();
        private static Selected OnSelected;
        
        private Subscription<GameEventType> gameEventSubscription;
        
        private void OnValidate()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            color = spriteRenderer.color;
            
            if (vertexController == null)
            {
                vertexController = transform.GetComponentInChildren<GridItemVertexController>();
            }
        }

        private void OnEnable()
        {
            OnSelected += Deselect;

            gameEventSubscription = PublisherSubscriber.Subscribe<GameEventType>(GameEventHandler);
        }

        private void OnDisable()
        {
            color = Color.white;
            spriteRenderer.color = color;
            Deselect();
            OnSelected -= Deselect;
            
            PublisherSubscriber.Unsubscribe(gameEventSubscription);
        }

        private void ShowParticle()
        {
            var particleObject = ObjectPooler.SharedInstance.GetPooledObject("Cell Particle");
            if (!particleObject.GetComponent<CellParticleController>(out var particleController)) return;
            particleObject.SetActive(true);
            particleController.Play(transform.position, color);
        }

        private void Update()
        {
            gameObject.name = $"GridItem [{position.rowPosition}, {position.columnPosition}]";
        }

        public int Score()
        {
            ShowParticle();
            gameObject.SetActive(false);
            return GameManager.Score;
        }
        
        public void Deselect()
        {
            onDeselect.Invoke();
        }

        public void Move(Vector3 worldPosition, float duration = 0.25f)
        {
            transform.DOMove(worldPosition, duration);
        }

        public void Move(Vector3 from, Vector3 to, float duration = 0.25f)
        {
            transform.position = from;
            transform.DOMove(to, duration);
        }
        
        public bool SetColor(Color newColor, bool isCheck = true)
        {
            if (!isCheck)
            {
                color = newColor;
                spriteRenderer.color = color;
                return true;
            }
            
            var sameColorCount = 0;
            foreach (var neighbor in neighbors)
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (neighbor.direction)
                {
                    case NeighborDirection.NorthWest when neighbor.item == null:
                        continue;
                    case NeighborDirection.NorthWest:
                    {
                        if (neighbor.item.color == newColor)
                        {
                            sameColorCount++;
                        }

                        break;
                    }
                    case NeighborDirection.North when neighbor.item == null:
                        continue;
                    case NeighborDirection.North:
                    {
                        if (neighbor.item.color == newColor)
                        {
                            sameColorCount++;
                        }

                        break;
                    }
                    case NeighborDirection.NorthEast when neighbor.item == null:
                        continue;
                    case NeighborDirection.NorthEast:
                    {
                        if (neighbor.item.color == newColor)
                        {
                            sameColorCount++;
                        }

                        break;
                    }
                    case NeighborDirection.SouthEast when neighbor.item == null:
                        continue;
                    case NeighborDirection.SouthEast:
                    {
                        if (neighbor.item.color == newColor)
                        {
                            sameColorCount++;
                        }
                        
                        break;
                    }
                    case NeighborDirection.SouthWest when neighbor.item == null:
                        continue;
                    case NeighborDirection.SouthWest:
                    {
                        if (neighbor.item.color == newColor)
                        {
                            sameColorCount++;
                        }
                        
                        break;
                    }
                }
            }

            if (sameColorCount >= 2)
            {
                return false;
            }
            color = newColor;
            spriteRenderer.color = color;
            return true;
        }

        public int CheckPossibleMoves()
        {
            possibleMoves = new List<PossibleMoveData> {new PossibleMoveData() {color = color, count = 1}};

            foreach (var neighbor in neighbors)
            {
                if (neighbor.item == null) continue;
                var isFound = false;
                for (var i = 0; i < possibleMoves.Count; i++)
                {
                    var possibleMove = possibleMoves[i];
                    if (possibleMove.color != neighbor.item.color) continue;
                    possibleMove.AddCount();
                    possibleMoves[i] = possibleMove;
                    isFound = true;
                    break;
                }

                if (!isFound)
                {
                    possibleMoves.Add(new PossibleMoveData()
                    {
                        color = neighbor.item.color,
                        count = 1
                    });
                }
            }
            
            var northNeighbor = neighbors[0];
            var southNeighbor = neighbors[3];
            
            var northEastNeighbor = neighbors[1];
            var southWestNeighbor = neighbors[4];

            var southEastNeighbor = neighbors[2];
            var northWestNeighbor = neighbors[5];
            foreach (var neighbor in neighbors)
            {
                switch (neighbor.direction)
                {
                    case NeighborDirection.North:
                        northNeighbor = neighbor;
                        break;
                    case NeighborDirection.South:
                        southNeighbor = neighbor;
                        break;
                    case NeighborDirection.NorthEast:
                        northEastNeighbor = neighbor;
                        break;
                    case NeighborDirection.SouthEast:
                        southEastNeighbor = neighbor;
                        break;
                    case NeighborDirection.SouthWest:
                        southWestNeighbor = neighbor;
                        break;
                    case NeighborDirection.NorthWest:
                        northWestNeighbor = neighbor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (northNeighbor.item != null && southNeighbor.item != null)
            {
                var neighborColors = new List<Color> {northNeighbor.item.color, southNeighbor.item.color};

                var isColorsSame = true;
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var neighborColor in neighborColors)
                {
                    if (color != neighborColor)
                    {
                        isColorsSame = false;
                    }
                }

                if (isColorsSame)
                {
                    ErasePossibleMove(color);
                }
            }

            if (northEastNeighbor.item != null && southWestNeighbor.item != null)
            {
                var neighborColors = new List<Color> {northEastNeighbor.item.color, southWestNeighbor.item.color};

                var isColorsSame = true;
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var neighborColor in neighborColors)
                {
                    if (color != neighborColor)
                    {
                        isColorsSame = false;
                    }
                }

                if (isColorsSame)
                {
                    ErasePossibleMove(color);
                }
            }
            
            if (southEastNeighbor.item != null && northWestNeighbor.item != null)
            {
                var neighborColors = new List<Color> {southEastNeighbor.item.color, northWestNeighbor.item.color};

                var isColorsSame = true;
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var neighborColor in neighborColors)
                {
                    if (color != neighborColor)
                    {
                        isColorsSame = false;
                    }
                }

                if (isColorsSame)
                {
                    ErasePossibleMove(color);
                }
            }

            if (northNeighbor.item != null && southEastNeighbor.item != null && southWestNeighbor.item != null)
            {
                var neighborColors = new List<Color>
                {
                    northNeighbor.item.color,
                    southEastNeighbor.item.color,
                    southWestNeighbor.item.color
                };

                var isColorsSame = true;
                foreach (var neighborColor in neighborColors.Where(neighborColor => neighborColors[0] != neighborColor))
                {
                    isColorsSame = false;
                }

                if (isColorsSame)
                {
                    ErasePossibleMove(neighborColors[0]);
                }
            }

            if (southNeighbor.item != null && northEastNeighbor.item != null && northWestNeighbor.item != null)
            {
                var neighborColors = new List<Color>
                {
                    southNeighbor.item.color,
                    northEastNeighbor.item.color,
                    northWestNeighbor.item.color
                };
                
                var isColorsSame = true;
                foreach (var neighborColor in neighborColors.Where(neighborColor => neighborColors[0] != neighborColor))
                {
                    isColorsSame = false;
                }

                if (isColorsSame)
                {
                    ErasePossibleMove(neighborColors[0]);
                }
            }
            
            possibleMoveCount = 0;
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var possibleMove in possibleMoves)
            {
                if (possibleMove.count >= 3)
                {
                    possibleMoveCount++;
                }
            }
            
            return possibleMoveCount;
        }

        private void ErasePossibleMove(Color possibleColor)
        {
            for (var i = 0; i < possibleMoves.Count; i++)
            {
                if (possibleMoves[i].color != possibleColor) continue;
                var temp = possibleMoves[i];
                temp.count -= 3;
                possibleMoves[i] = temp;

                Debug.Log($"{gameObject.name} : Possible move is erased.");
                break;
            }
        }

        public List<GridPosition> CheckScore()
        {
            var positions = new List<GridPosition>();
            var possibleSameColors = new List<Neighbor.Neighbor>();
            for (var i = 0; i < neighbors.Count; i++)
            {
                possibleSameColors.Clear();
                
                var currentNeighbor = neighbors[i];
                if (currentNeighbor.item == null) continue;
                if (currentNeighbor.item.color == color)
                {
                    possibleSameColors.Add(currentNeighbor);
                    
                    var previousNeighbor = neighbors[i - 1 < 0 ? neighbors.Count - 1 : i - 1];
                    if (previousNeighbor.item != null)
                    {
                        if (previousNeighbor.item.color == color)
                        {
                            possibleSameColors.Add(previousNeighbor);        
                        }
                    }

                    var nextNeighbor = neighbors[(i + 1) % neighbors.Count];
                    if (nextNeighbor.item != null)
                    {
                        if (nextNeighbor.item.color == color)
                        {
                            possibleSameColors.Add(nextNeighbor);
                        }
                    }
                }

                if (possibleSameColors.Count <= 1) continue;
                positions.Add(position);
                foreach (var possibleSameColor in possibleSameColors)
                {
                    positions.Add(possibleSameColor.item.position);
                }
                break;
            }

            return positions;
        }
        
        private void GameEventHandler(GameEventType gameEventType)
        {
            if (gameEventType == GameEventType.CheckNeighbors)
            {
                FindNeighbors();
            }

            if (gameEventType == GameEventType.UnselectAll)
            {
                Deselect();
            }

            if (gameEventType == GameEventType.MovePosition)
            {
                Move();
            }

            if (gameEventType == GameEventType.GameOver)
            {
                ShowParticle();
                gameObject.SetActive(false);
            }
        }

        private void Move()
        {
            Move(GridSystem.Instance.FindPosition(
                position.rowPosition,
                position.columnPosition));
        }
        
        private void Select()
        {
            onSelect?.Invoke();
        }
        
        public void Select(Vector3 inputPosition)
        {
            OnSelected?.Invoke();
            
            var selectedNeighbors = new List<Neighbor.Neighbor>();
            var direction = vertexController.Find(inputPosition);

            while (true)
            {
                selectedNeighbors.Clear();
                
                var isFound = false;
                switch (direction)
                {
                    case VertexDirection.NorthEast:
                        // Check for both North & NorthEast Neighbors
                        foreach (var neighbor in neighbors)
                        {
                            if (neighbor.direction == NeighborDirection.North)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }

                                selectedNeighbors.Add(neighbor);
                            }

                            if (neighbor.direction == NeighborDirection.NorthEast)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }

                                selectedNeighbors.Add(neighbor);
                            }

                            isFound = true;
                        }
                        break;
                    case VertexDirection.East:
                        // Check for both NorthEast & SouthEast Neighbors
                        foreach (var neighbor in neighbors)
                        {
                            if (neighbor.direction == NeighborDirection.NorthEast)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            if (neighbor.direction == NeighborDirection.SouthEast)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            isFound = true;
                        }
                        break;
                    case VertexDirection.SouthEast:
                        // Check for both SouthEast & South Neighbors
                        foreach (var neighbor in neighbors)
                        {
                            if (neighbor.direction == NeighborDirection.SouthEast)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            if (neighbor.direction == NeighborDirection.South)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            isFound = true;
                        }
                        break;
                    case VertexDirection.SouthWest:
                        // Check for both South & SouthWest Neighbors
                        foreach (var neighbor in neighbors)
                        {
                            if (neighbor.direction == NeighborDirection.South)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            if (neighbor.direction == NeighborDirection.SouthWest)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            isFound = true;
                        }
                        break;
                    case VertexDirection.West:
                        // Check for both SouthWest & NorthWest Neighbors
                        foreach (var neighbor in neighbors)
                        {
                            if (neighbor.direction == NeighborDirection.SouthWest)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            if (neighbor.direction == NeighborDirection.NorthWest)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            isFound = true;
                        }
                        break;
                    case VertexDirection.NorthWest:
                        // Check for both NorthWest & North Neighbors
                        foreach (var neighbor in neighbors)
                        {
                            if (neighbor.direction == NeighborDirection.NorthWest)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            if (neighbor.direction == NeighborDirection.North)
                            {
                                if (neighbor.item == null)
                                {
                                    direction = (VertexDirection) (((int) direction + 1) % Enum.GetNames(typeof(VertexDirection)).Length);
                                    isFound = false;
                                    break;
                                }
                                
                                selectedNeighbors.Add(neighbor);
                            }

                            isFound = true;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (isFound)
                {
                    break;
                }
            }

            onSelect?.Invoke();
            foreach (var selectedNeighbor in selectedNeighbors)
            {
                selectedNeighbor.item.Select();
            }
            
            var selectedGridItems = new SelectedGridItems()
            {
                gridPosition = Position,
                inputPosition = inputPosition,
                root = this,
                neighbor1 = selectedNeighbors[0].item,
                neighbor1Direction = selectedNeighbors[0].direction,
                neighbor2 = selectedNeighbors[1].item,
                neighbor2Direction = selectedNeighbors[1].direction
            };
            
            PublisherSubscriber.Publish(selectedGridItems);
        }

        private void FindNeighbors()
        {
            if (neighbors == null)
            {
                neighbors = new List<Neighbor.Neighbor>();
            }
            neighbors.Clear();

            var directions = Enum.GetValues(typeof(NeighborDirection));
            foreach (var direction in directions)
            {
                var neighbor = new Neighbor.Neighbor {direction = (NeighborDirection) direction};
                switch (neighbor.direction)
                {
                    case NeighborDirection.North:
                        if (position.rowPosition - 1 < 0)
                        {
                            neighbor.item = null;
                        }
                        else
                        {
                            neighbor.item = GridSystem.Instance.FindGridItem(
                                position.rowPosition - 1, 
                                position.columnPosition);
                        }
                        break;
                    case NeighborDirection.NorthEast:
                        if (position.columnPosition + 1 > GridSystem.Instance.ColumnCount - 1)
                        {
                            neighbor.item = null;
                        }
                        else
                        {
                            if (position.columnPosition % 2 == 0)
                            {
                                if (position.rowPosition - 1 < 0)
                                {
                                    neighbor.item = null;
                                }
                                else
                                {
                                    neighbor.item = GridSystem.Instance.FindGridItem(
                                        position.rowPosition - 1,
                                        position.columnPosition + 1);
                                }
                            }
                            else
                            {
                                neighbor.item = GridSystem.Instance.FindGridItem(
                                    position.rowPosition,
                                    position.columnPosition + 1);
                            }
                        }
                        break;
                    case NeighborDirection.SouthEast:
                        if (position.columnPosition + 1 > GridSystem.Instance.ColumnCount - 1)
                        {
                            neighbor.item = null;
                        }
                        else
                        {
                            if (position.columnPosition % 2 == 0)
                            {
                                neighbor.item = GridSystem.Instance.FindGridItem(
                                    position.rowPosition,
                                    position.columnPosition + 1);
                            }
                            else
                            {
                                if (position.rowPosition + 1 > GridSystem.Instance.RowCount - 1)
                                {
                                    neighbor.item = null;
                                }
                                else
                                {
                                    neighbor.item = GridSystem.Instance.FindGridItem(
                                        position.rowPosition + 1,
                                        position.columnPosition + 1);
                                }
                            }
                        }
                        break;
                    case NeighborDirection.South:
                        if (position.rowPosition + 1 > GridSystem.Instance.RowCount - 1)
                        {
                            neighbor.item = null;
                        }
                        else
                        {
                            neighbor.item = GridSystem.Instance.FindGridItem(
                                position.rowPosition + 1,
                                position.columnPosition);
                        }
                        break;
                    case NeighborDirection.SouthWest:
                        if (position.columnPosition - 1 < 0)
                        {
                            neighbor.item = null;
                        }
                        else
                        {
                            if (position.columnPosition % 2 == 0)
                            {
                                neighbor.item = GridSystem.Instance.FindGridItem(
                                    position.rowPosition,
                                    position.columnPosition - 1);
                            }
                            else
                            {
                                if (position.rowPosition + 1 > GridSystem.Instance.RowCount - 1)
                                {
                                    neighbor.item = null;
                                }
                                else
                                {
                                    neighbor.item = GridSystem.Instance.FindGridItem(
                                        position.rowPosition + 1,
                                        position.columnPosition - 1);
                                }
                            }
                        }
                        break;
                    case NeighborDirection.NorthWest:
                        if (position.columnPosition - 1 < 0)
                        {
                            neighbor.item = null;
                        }
                        else
                        {
                            if (position.columnPosition % 2 == 0)
                            {
                                if (position.rowPosition - 1 < 0)
                                {
                                    neighbor.item = null;
                                }
                                else
                                {
                                    neighbor.item = GridSystem.Instance.FindGridItem(
                                        position.rowPosition - 1,
                                        position.columnPosition - 1);
                                }
                            }
                            else
                            {
                                neighbor.item = GridSystem.Instance.FindGridItem(
                                    position.rowPosition,
                                    position.columnPosition - 1);
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                neighbors.Add(neighbor);
            }
        }
    }
}
