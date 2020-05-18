using TMPro;
using UnityEngine;

namespace UI.Abstract
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class ShowUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        protected TextMeshProUGUI Text => text;
        
        private void OnValidate()
        {
            if (text == null)
            {
                text = GetComponent<TextMeshProUGUI>();
            }
        }

        private void Update()
        {
            Show();
        }

        protected abstract void Show();
    }
}