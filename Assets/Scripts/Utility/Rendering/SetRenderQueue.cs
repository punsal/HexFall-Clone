using UnityEngine;

namespace Utility.Rendering
{
    [AddComponentMenu("Rendering/SetRenderQueue")]
    [ExecuteAlways]
    public class SetRenderQueue : MonoBehaviour
    {
        private Renderer objectRenderer;
        [SerializeField] protected int[] renderQueues = {3000};

        protected void Awake()
        {
            objectRenderer = GetComponent<Renderer>();

            var materials = objectRenderer.sharedMaterials;
            for (var i = 0; i < materials.Length && i < renderQueues.Length; ++i)
                materials[i].renderQueue = renderQueues[i];
        }
    }
}