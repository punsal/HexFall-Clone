using EventArguments;
using TMPro;
using UnityEngine;
using Utility.System.Publisher_Subscriber_System;
using Random = UnityEngine.Random;

namespace Item
{
    public class BombItem : MonoBehaviour
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        private static bool IsBombEnabled = false;
        #pragma warning disable 649
        [SerializeField] private GameObject bombCanvas;
        [SerializeField] private TextMeshProUGUI text;
        #pragma warning restore 649
        
        // ReSharper disable once RedundantDefaultMemberInitializer
        [SerializeField] private bool isBomb = false;
        [SerializeField] private int countdown;

        private Subscription<GameEventType> gameEventSubscription;
        
        private void OnEnable()
        {
            bombCanvas.SetActive(false);

            if (GameManager.TotalScore < GameManager.BombCount * GameManager.BombStep) return;
            if (IsBombEnabled) return;
            IsBombEnabled = true;
            isBomb = true;
            GameManager.BombCount++;
            bombCanvas.SetActive(true);
            text.text = (countdown = Random.Range(8, 13)).ToString();

            gameEventSubscription = PublisherSubscriber.Subscribe<GameEventType>(GameEventHandler);
        }

        private void GameEventHandler(GameEventType gameEventType)
        {
            if (gameEventType != GameEventType.UpdateMoveCount) return;
            countdown--;
            text.text = countdown.ToString();
            if (countdown == 0)
            {
                PublisherSubscriber.Publish(GameEventType.GameOver);
            }
        }

        private void OnDisable()
        {
            if (!isBomb) return;
            IsBombEnabled = false;
            isBomb = false;
            
            PublisherSubscriber.Unsubscribe(gameEventSubscription);
        }
    }
}
