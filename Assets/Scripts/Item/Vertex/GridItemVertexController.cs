using System.Collections.Generic;
using Item.Vertex.Direction;
using UnityEngine;
using Utility.Extension;

namespace Item.Vertex
{
    public class GridItemVertexController : MonoBehaviour
    {
        [SerializeField] private List<GridItemVertex> vertices;

        private void OnValidate()
        {
            vertices = transform.GetComponentsListInChildren<GridItemVertex>();
        }

        public VertexDirection Find(Vector3 position)
        {
            var minDistance = Vector3.Distance(position, vertices[0].Position);
            var index = 0;
            for (var i = 0; i < vertices.Count; i++)
            {
                var distance = Vector3.Distance(position, vertices[i].Position);
                if (!(minDistance > distance)) continue;
                minDistance = distance;
                index = i;
            }

            return vertices[index].VertexDirection;
        }
    }
}
