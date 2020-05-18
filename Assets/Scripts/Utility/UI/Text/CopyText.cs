using TMPro;
using UnityEngine;

namespace Utility.UI.Text
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteAlways]
    public class CopyText : MonoBehaviour
    {
        private TextMeshProUGUI text;
#pragma warning disable 649
        [SerializeField] private TextMeshProUGUI textToCopy;
#pragma warning restore 649

        private void OnValidate()
        {
        }

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (textToCopy == null) return;
            text.text = textToCopy.text;
        }
    }
}