using Item.Vertex.Direction;
using UnityEngine;

namespace Item.Vertex
{
    public class GridItemVertex : MonoBehaviour
    {
        [SerializeField] private VertexDirection vertexDirection = VertexDirection.NorthEast;
        public VertexDirection VertexDirection => vertexDirection;

        public Vector3 Position => transform.position;
    }
}
