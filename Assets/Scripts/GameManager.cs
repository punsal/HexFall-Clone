using System;
using System.Collections;
using EventArguments;
using Grid;
using Manager;
using UnityEngine;
using Utility.System.Publisher_Subscriber_System;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    #pragma warning disable 649
    [SerializeField] private GameObject inputPanel;
    [SerializeField] private GameObject gameOverPanel;
    #pragma warning restore 649
    
    [Header("Debug")]
    // ReSharper disable once RedundantDefaultMemberInitializer
    // ReSharper disable once NotAccessedField.Local
    [SerializeField] private int totalPossibleMoveCount = 0;
    
    [Header("Selected Debug")]
    [SerializeField] private SelectedGridItems currentSelectedGridItems;
    
    public static int BombCount = 1;
    public const int BombStep = 1000;
    // ReSharper disable once RedundantDefaultMemberInitializer
    public static int TotalMoveCount = 0;
    // ReSharper disable once RedundantDefaultMemberInitializer
    public static int TotalScore = 0;
    public const int Score = 5;
    // ReSharper disable once RedundantDefaultMemberInitializer
    private static bool IsScored = false;
    public static bool IsChecking = false;
    
    private Subscription<SelectedGridItems> selectedGridItemsSubscription;
    private Subscription<SwipeDirection> swipeSubscription;
    private Subscription<GameEventType> gameEventSubscription;

    private void OnEnable()
    {
        selectedGridItemsSubscription = PublisherSubscriber.Subscribe<SelectedGridItems>(OnSelectedGridItemsHandler);
        swipeSubscription = PublisherSubscriber.Subscribe<SwipeDirection>(SwipeHandler);
        gameEventSubscription = PublisherSubscriber.Subscribe<GameEventType>(GameEventHandler);
    }

    private void OnDisable()
    {
        PublisherSubscriber.Unsubscribe(selectedGridItemsSubscription);
        PublisherSubscriber.Unsubscribe(swipeSubscription);
        PublisherSubscriber.Unsubscribe(gameEventSubscription);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        BombCount = 1;
        TotalMoveCount = 0;
        TotalScore = 0;
        
        GridSystem.Instance.Generate();
        var isColorized = false;
        while (!isColorized)
        {
            isColorized = GridSystem.Instance.Colorize();
        }

        totalPossibleMoveCount = GridSystem.Instance.GetPossibleMoveCount();
        
        inputPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public static void AddScore(int score)
    {
        IsScored = true;
        TotalScore += score;
    }
    
    private void OnSelectedGridItemsHandler(SelectedGridItems selectedGridItems)
    {
        currentSelectedGridItems = selectedGridItems;
    }

    private void SwipeHandler(SwipeDirection swipeDirection)
    {
        StartCoroutine(SwapItems(swipeDirection));
    }

    private IEnumerator SwapItems(SwipeDirection swipeDirection)
    {
        if (currentSelectedGridItems.root == null) yield break;
        
        inputPanel.SetActive(false);
        
        IsScored = false;
        var swappingCount = 0;
        while (!IsScored)
        {
            swappingCount++;
            
            if (currentSelectedGridItems.neighbor1.Position.rowPosition + 1 == currentSelectedGridItems.root.Position.rowPosition)
            {
                if (((int) currentSelectedGridItems.neighbor1Direction + 1) % 6 != (int) currentSelectedGridItems.neighbor2Direction)
                {
                    Debug.Log("Weird things are happening.");
                    var newCurrentSelectedItems = currentSelectedGridItems;
                    newCurrentSelectedItems.neighbor2 = currentSelectedGridItems.neighbor1;
                    newCurrentSelectedItems.neighbor1 = currentSelectedGridItems.neighbor2;

                    currentSelectedGridItems = newCurrentSelectedItems;
                }
            }
            var rootPosition = currentSelectedGridItems.root.transform.position;
            var neighbor1Position = currentSelectedGridItems.neighbor1.transform.position;
            var neighbor2Position = currentSelectedGridItems.neighbor2.transform.position;
            
            switch (swipeDirection)
            {
                case SwipeDirection.Left:
                    GridSystem.Instance.Swap(
                        currentSelectedGridItems.root.Position,
                        currentSelectedGridItems.neighbor2.Position);
                    
                    GridSystem.Instance.Swap(
                        currentSelectedGridItems.neighbor1.Position,
                        currentSelectedGridItems.neighbor2.Position);
                    
                    currentSelectedGridItems.root.Move(neighbor2Position);
                    currentSelectedGridItems.neighbor1.Move(rootPosition);
                    currentSelectedGridItems.neighbor2.Move(neighbor1Position);
                    break;
                case SwipeDirection.Right:
                    GridSystem.Instance.Swap(
                        currentSelectedGridItems.root.Position,
                        currentSelectedGridItems.neighbor1.Position);
                
                    GridSystem.Instance.Swap(
                        currentSelectedGridItems.neighbor1.Position,
                        currentSelectedGridItems.neighbor2.Position);
                
                    currentSelectedGridItems.root.Move(neighbor1Position);
                    currentSelectedGridItems.neighbor1.Move(neighbor2Position);
                    currentSelectedGridItems.neighbor2.Move(rootPosition);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(swipeDirection), swipeDirection, null);
            }

            yield return new WaitForSeconds(0.25f);
                
            PublisherSubscriber.Publish(GameEventType.CheckNeighbors);
        
            GridSystem.Instance.CheckScore();
        
            yield return new WaitForSeconds(1f);
        
            while (IsChecking)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }

            GridSystem.Instance.SelectGridItem(currentSelectedGridItems.gridPosition, currentSelectedGridItems.inputPosition);
                    
            if (swappingCount == 3)
            {
                break;
            }
        }

        inputPanel.SetActive(true);
        
        if (!IsScored) yield break;
        TotalMoveCount++;
        PublisherSubscriber.Publish(GameEventType.UpdateMoveCount);
        totalPossibleMoveCount = GridSystem.Instance.GetPossibleMoveCount();    
        CheckPossibleMoveCount();
    }

    private void CheckPossibleMoveCount()
    {
        if (totalPossibleMoveCount == 0)
        {
            PublisherSubscriber.Publish(GameEventType.GameOver);
        }
    }
    
    private void GameEventHandler(GameEventType gameEventType)
    {
        // ReSharper disable once InvertIf
        if (gameEventType == GameEventType.GameOver)
        {
            inputPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }
}
