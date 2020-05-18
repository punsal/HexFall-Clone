using UnityEngine;

namespace Utility.Extension
{
    public static class MeshExtension
    {
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public static Vector3[] FindWorldLocations(this Mesh mesh, Transform transform)
        {
            var localToWorld = transform.localToWorldMatrix;
            var meshVertices = mesh.vertices;
            var worldLocations = new Vector3[meshVertices.Length];
            for (var i = 0; i < worldLocations.Length; i++)
                worldLocations[i] = localToWorld.MultiplyPoint3x4(meshVertices[i]);

            return worldLocations;
        }
    }
}