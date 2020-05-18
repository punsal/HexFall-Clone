using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventArguments;
using Grid.Position;
using Item;
using UnityEngine;
using Utility.Extension;
using Utility.System.Publisher_Subscriber_System;
using Random = UnityEngine.Random;

namespace Grid
{
    [ExecuteAlways]
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance;
        
        [Header("Screen View")]
        [SerializeField] private Camera cameraMain;
        [SerializeField] private float viewPercentage = 0.85f;

        [Header("Grid Item Properties")]
        #pragma warning disable 649
        [SerializeField] private GameObject gridItemPrefab;
        #pragma warning restore 649
        [SerializeField] private float gridItemWidth = 1f;
        [SerializeField] private float gridItemHeight = 1f;
        [SerializeField] private float scaleFactor = 1f;

        [Header("Grid Properties")]
        [SerializeField] private int gridColumnCount = 8;
        [SerializeField] private int gridRowCount = 9;
        public int ColumnCount => gridColumnCount;
        public int RowCount => gridRowCount;

        [Header("Color Settings")]
        #pragma warning disable 649
        [SerializeField] private List<GridColorData> colorData;
        #pragma warning restore 649
        
        [Header("Screen View Debug")]
        [SerializeField] private Vector3 screenOriginPosition = Vector3.zero;
        [SerializeField] private Vector3 screenLeftPosition = Vector3.left;
        [SerializeField] private Vector3 screenRightPosition = Vector3.right;
        [SerializeField] private Vector3 screenUpPosition = Vector3.up;
        [SerializeField] private Vector3 screenDownPosition = Vector3.down;
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private float screenWidth = 0f;
        // ReSharper disable once RedundantDefaultMemberInitializer
        // ReSharper disable once NotAccessedField.Local
        [SerializeField] private float screenHeight = 0f;
        [SerializeField] private Vector3 startingPosition = Vector3.zero;
        
        [Header("Grid Debug")]
        [SerializeField] private List<GridItem> gridItems;
        [SerializeField] private List<Vector3> gridItemTransforms;
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private float totalGridWidth = 0f;
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private float totalGridHeight = 0f;

        private void OnValidate()
        {
            if (cameraMain == null)
            {
                cameraMain = Camera.main;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Generate()
        {
            ClearGridItems();
            
            ClearRemainingChildren();
            
            CalculateScreenView();
            CalculateScaleFactor();
            CalculateStartingPosition();

            if (gridItemTransforms == null)
            {
                gridItemTransforms = new List<Vector3>();
            }
            gridItemTransforms.Clear();

            for (var i = 0; i < gridRowCount; i++)
            {
                for (var j = 0; j < gridColumnCount; j++)
                {
                    var tempItem = Instantiate(gridItemPrefab);
                    tempItem.name = $"GridItem [{i},{j}]";

                    var tempItemTransform = tempItem.transform;
                    tempItemTransform.position = startingPosition + CalculatePosition(i,j);
                    tempItemTransform.localScale = Vector3.one * scaleFactor;
                    tempItemTransform.SetParent(transform);
                    
                    gridItemTransforms.Add(tempItemTransform.position);

                    var tempGridItem = tempItem.GetComponent<GridItem>();
                    tempGridItem.Deselect();
                    tempGridItem.Position = new GridPosition()
                    {
                        rowPosition = i,
                        columnPosition = j
                    };
                    
                    gridItems.Add(tempGridItem);
                }
            }
            
            PublisherSubscriber.Publish(GameEventType.CheckNeighbors);
        }

        public bool Colorize()
        {
            for (var i = 0; i < colorData.Count; i++)
            {
                var temp = colorData[i];
                temp.ResetCounter();
                colorData[i] = temp;
            }
            
            var freeCountTypedColors = 0;
            var totalCount = 0;
            foreach (var data in colorData)
            {
                if (data.colorType == GridColorType.FixedCount)
                {
                    totalCount += data.count;
                }
                else
                {
                    freeCountTypedColors++;
                }
            }

            if (totalCount < gridItems.Count)
            {
                if (freeCountTypedColors < 3)
                {
                    Debug.LogError("Total sum of FixedCount typed colorData count must be equal " +
                                   "or greater to grid count or there must be at least two FreeCount typed colorData.");
                    return false;
                }
            }

            var isColorSet = false;
            foreach (var item in gridItems)
            {
                isColorSet = false;
                var selectedColors = new List<GridColorData>();
                while (!isColorSet)
                {
                    if (selectedColors.Count == colorData.Count)
                    {
                        break;
                    }
                    var randomIndex = Random.Range(0, colorData.Count);
                    var randomColorData = colorData[randomIndex];
                    if (selectedColors.Contains(randomColorData))
                    {
                        Debug.Log("Same color selected.");
                    }
                    else
                    {
                        selectedColors.Add(randomColorData);
                    }
                    switch (randomColorData.colorType)
                    {
                        case GridColorType.FixedCount:
                            if (randomColorData.counter < randomColorData.count)
                            {
                                isColorSet = item.SetColor(randomColorData.color);
                                if (isColorSet)
                                {
                                    randomColorData.counter++;
                                    colorData[randomIndex] = randomColorData;
                                }
                            }
                            break;
                        case GridColorType.FreeCount:
                            isColorSet = item.SetColor(randomColorData.color);
                            if (isColorSet)
                            {
                                randomColorData.counter++;
                                colorData[randomIndex] = randomColorData;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (!isColorSet)
                {
                    break;
                }
            }

            return isColorSet;
        }
        
        public GridItem FindGridItem(int rowIndex, int columnIndex)
        {
            var index = rowIndex * gridColumnCount + columnIndex;
            // ReSharper disable once InvertIf
            return index > gridItems.Count - 1 ? null : gridItems[index];
        }

        public int GetPossibleMoveCount()
        {
            return gridItems.Sum(item => item.CheckPossibleMoves());
        }

        public void Swap(GridPosition position1, GridPosition position2)
        {
            var position1Index = position1.rowPosition * gridColumnCount + position1.columnPosition;
            var position2Index = position2.rowPosition * gridColumnCount + position2.columnPosition;

            var itemAtPosition1GridPosition = gridItems[position1Index].Position;
            var itemAtPosition2GridPosition = gridItems[position2Index].Position;
            
            var temp = gridItems[position1Index];
            gridItems[position1Index] = gridItems[position2Index];
            gridItems[position1Index].Position = itemAtPosition1GridPosition;
            
            gridItems[position2Index] = temp;
            gridItems[position2Index].Position = itemAtPosition2GridPosition;
        }

        public void SelectGridItem(GridPosition position, Vector3 inputPosition)
        {
            var item = FindGridItem(position.rowPosition, position.columnPosition);
            item.Select(inputPosition);
        }

        public void CheckScore()
        {
            var isScored = false;
            do
            {
                isScored = false;
                foreach (var item in gridItems)
                {
                    if (!item.gameObject.activeInHierarchy)
                    {
                        continue;
                    }
                    var positions = item.CheckScore();
                    if (positions.Count <= 2) continue;
                    foreach (var gridPosition in positions)
                    {
                        var temp = FindGridItem(gridPosition.rowPosition, gridPosition.columnPosition);
                        GameManager.AddScore(temp.Score());
                    }
                    isScored = true;
                    break;
                }
            } while (isScored);

            StartCoroutine(CheckGrid());
        }

        private IEnumerator CheckGrid()
        {
            PublisherSubscriber.Publish(GameEventType.UnselectAll);
            GameManager.IsChecking = true;
            var isGridReorganized = false;
            
            for (var columnIndex = gridColumnCount - 1; columnIndex >= 0; columnIndex--)
            {
                for (var rowIndex = gridRowCount - 1; rowIndex >= 0; rowIndex--)
                {
                    var currentGridItem = FindGridItem(rowIndex, columnIndex);
                    if (currentGridItem.gameObject.activeInHierarchy)
                    {
                        continue;
                    }
                    if (rowIndex - 1 < 0) continue;
                    var nextGridItem = FindGridItem(rowIndex - 1, columnIndex);
                    if (!nextGridItem.gameObject.activeInHierarchy) continue;
                    Swap(currentGridItem.Position, nextGridItem.Position);
                    currentGridItem.Move(FindPosition(
                        currentGridItem.Position.rowPosition,
                        currentGridItem.Position.columnPosition));
                    nextGridItem.Move(FindPosition(
                        nextGridItem.Position.rowPosition,
                        nextGridItem.Position.columnPosition));
                    yield return new WaitForSeconds(0.25f);
                    rowIndex = gridRowCount;
                }
            }
            
            for (var columnIndex = gridColumnCount - 1; columnIndex >= 0; columnIndex--)
            {
                for (var rowIndex = gridRowCount - 1; rowIndex >= 0; rowIndex--)
                {
                    var currentGridItem = FindGridItem(rowIndex, columnIndex);
                    if (!currentGridItem.gameObject.activeInHierarchy)
                    {
                        isGridReorganized = true;
                        currentGridItem.gameObject.SetActive(true);
                        currentGridItem.SetColor(colorData[Random.Range(0, colorData.Count)].color, false);
                        currentGridItem.Position.SetPosition(rowIndex, columnIndex);
                        currentGridItem.Move(
                            currentGridItem.transform.position.SetY(screenUpPosition.y),
                            FindPosition(currentGridItem.Position.rowPosition,
                                currentGridItem.Position.columnPosition),
                            1f);
                        yield return new WaitForSeconds(1f);
                    }
                }
            }
            
            PublisherSubscriber.Publish(GameEventType.CheckNeighbors);
            
            if (isGridReorganized)
            {
                CheckScore();
                yield break;
            }

            GameManager.IsChecking = false;
        }
        
        private Vector3 CalculatePosition(int rowIndex, int columnIndex)
        {
            return new Vector3(
                columnIndex * (3f * (gridItemWidth * scaleFactor * 0.5f) * 0.5f),
                columnIndex % 2 == 0 
                    ? -1f * rowIndex * (gridItemWidth * scaleFactor * 0.5f) * Mathf.Sqrt(3)
                    : -1f * (rowIndex * (gridItemWidth * scaleFactor * 0.5f) * Mathf.Sqrt(3) + gridItemWidth * scaleFactor * 0.5f * Mathf.Sqrt(3) * 0.5f),
                0f);
        }

        public Vector3 FindPosition(int rowIndex, int columnIndex)
        {
            var index = rowIndex * gridColumnCount + columnIndex;
            // ReSharper disable once InvertIf
            return index > gridItemTransforms.Count - 1 ? Vector3.zero : gridItemTransforms[index];
        }
        
        private void CalculateScreenView()
        {
            var cameraPosition = cameraMain.transform.position;
            screenOriginPosition = cameraMain.GetWorldPositionIn3D(new Vector2(
                    cameraMain.pixelWidth * 0.5f,
                    cameraMain.pixelHeight * 0.5f),
                -1f * cameraPosition.z);
            
            screenLeftPosition = cameraMain.GetWorldPositionIn3D(new Vector2(
                    0f,
                    cameraMain.pixelHeight * 0.5f),
                -1f * cameraPosition.z);
            screenRightPosition = cameraMain.GetWorldPositionIn3D(new Vector2(
                    cameraMain.pixelWidth,
                    cameraMain.pixelHeight * 0.5f),
                -1f * cameraPosition.z);
            screenWidth = Vector3.Distance(screenLeftPosition, screenRightPosition);

            screenUpPosition = cameraMain.GetWorldPositionIn3D(new Vector2(
                    cameraMain.pixelWidth * 0.5f,
                    cameraMain.pixelHeight),
                -1f * cameraPosition.z);
            screenDownPosition = cameraMain.GetWorldPositionIn3D(new Vector2(
                    cameraMain.pixelWidth * 0.5f,
                    0f),
                -1f * cameraPosition.z);
            screenHeight = Vector3.Distance(screenUpPosition, screenDownPosition);
        }

        private void CalculateScaleFactor()
        {
            var totalWidth = gridItemWidth;
            for (var i = 0; i < gridColumnCount - 1; i++)
            {
                totalWidth += 3f * gridItemWidth * 0.25f;
            }
            scaleFactor = (screenWidth * viewPercentage) / totalWidth;
            scaleFactor = scaleFactor > 1f ? 1f : scaleFactor;

            totalGridWidth = 0f;
            for (var i = 0; i < gridColumnCount - 1; i++)
            {
                totalGridWidth += 3f * (gridItemWidth * scaleFactor) * 0.25f;
            }
            totalGridHeight = (gridRowCount - 1) * (gridItemHeight * scaleFactor);
        }

        private void CalculateStartingPosition()
        {
            startingPosition = screenOriginPosition.AddX(-1f * (totalGridWidth * 0.5f)).AddY(totalGridHeight * 0.5f);
        }

        private void ClearRemainingChildren()
        {
            var remainingObjects = transform.GetComponentsListInChildren<Transform>();
            remainingObjects.RemoveAt(0);
            while (remainingObjects.Count > 0)
            {
                var temp = remainingObjects[0];
                remainingObjects.RemoveAt(0);
#if UNITY_EDITOR
                DestroyImmediate(temp.gameObject);
#else
                Destroy(temp.gameObject);
#endif
            }
        }

        private void ClearGridItems()
        {
            if (gridItems == null)
            {
                gridItems = new List<GridItem>();
            }

            while (gridItems.Count > 0)
            {
                var tempItem = gridItems[0];
                gridItems.RemoveAt(0);
#if UNITY_EDITOR
                DestroyImmediate(tempItem.gameObject);
#else
                Destroy(tempItem);
#endif
            }
        }
    }
}
