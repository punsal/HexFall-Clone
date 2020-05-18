using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utility.Extension;
using Utility.System.Publisher_Subscriber_System;
using Utility.UI.Information_UI.Data;
using Utility.UI.Information_UI.Type;

namespace Utility.UI.Information_UI
{
    public class InformationUIManager : MonoBehaviour
    {
        private Subscription<InformationUIData> changeInformationEventSubscription;

        [Header("Additional Events")]
#pragma warning disable 649
        [SerializeField]
        private UnityEvent onChange;
#pragma warning restore 649

        [Header("Directional Padding")] [SerializeField]
        private float padding = 100f;

        [Header("Text to Change")]
#pragma warning disable 649
        [SerializeField]
        private TextMeshProUGUI textInformation;
#pragma warning restore 649

        private void OnEnable()
        {
            changeInformationEventSubscription =
                PublisherSubscriber.Subscribe<InformationUIData>(ChangeInformationEventHandler);
        }

        private void OnDisable()
        {
            PublisherSubscriber.Unsubscribe(changeInformationEventSubscription);
        }

        private void ChangeInformationEventHandler(InformationUIData data)
        {
            switch (data.ChangeType)
            {
                case InformationUIChangeType.Immediately:
                    textInformation.text = data.Text;
                    break;
                case InformationUIChangeType.Animate:
                    var rectTransform = GetComponent<RectTransform>();
                    var currentPosition = rectTransform.anchoredPosition;
                    var topPosition = Vector2.zero;
                    var distance = Mathf.Abs(topPosition.y - currentPosition.y);
                    var destination = topPosition.AddY(distance + textInformation.rectTransform.sizeDelta.y + padding);

                    var dataDuration = Math.Abs(data.Duration) < 0.0001f ? 0.5f : data.Duration * 0.5f;
                    rectTransform.DOAnchorPos(destination, dataDuration)
                        .OnComplete(() =>
                        {
                            textInformation.text = data.Text;
                            rectTransform.DOAnchorPos(currentPosition, dataDuration);
                        });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            onChange.Invoke();
        }
    }
}