using UnityEngine;

namespace Utility.Extension
{
    public static class VectorExtension
    {
        public static Vector2 ToVector2(this Vector3 vector3, bool isFlatten = false)
        {
            switch (isFlatten)
            {
                case true:
                    return new Vector2(vector3.x, vector3.y);
                default:
                    return new Vector2(vector3.x, vector3.z);
            }
        }

        public static Vector3 ToVector3(this Vector2 vector2, bool isFlatten = false)
        {
            switch (isFlatten)
            {
                case true:
                    return new Vector3(vector2.x, 0f, vector2.y);
                default:
                    return new Vector3(vector2.x, vector2.y);
            }
        }

        public static Vector3 SetX(this Vector3 vector3, float? x)
        {
            return new Vector3(x ?? vector3.x, vector3.y, vector3.z);
        }

        public static Vector3 SetY(this Vector3 vector3, float? y)
        {
            return new Vector3(vector3.x, y ?? vector3.y, vector3.z);
        }

        public static Vector3 SetZ(this Vector3 vector3, float? z)
        {
            return new Vector3(vector3.x, vector3.y, z ?? vector3.z);
        }

        public static Vector3 SetVector3(this Vector3 vector3, float? x, float? y, float? z)
        {
            return new Vector3(x ?? vector3.x, y ?? vector3.y, z ?? vector3.z);
        }

        public static Vector3 AddX(this Vector3 vector3, float x)
        {
            return new Vector3(vector3.x + x, vector3.y, vector3.z);
        }
        
        public static Vector3 AddY(this Vector3 vector3, float y)
        {
            return new Vector3(vector3.x, vector3.y + y, vector3.z);
        }

        public static Vector3 AddZ(this Vector3 vector3, float z)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z + z);
        }
        
        public static Vector2 AddX(this Vector2 vector2, float x)
        {
            return new Vector2(vector2.x + x, vector2.y);
        }
        
        public static Vector2 AddY(this Vector2 vector2, float y)
        {
            return new Vector2(vector2.x, vector2.y + y);
        }

        public static Vector2 SetX(this Vector2 vector2, float x)
        {
            return new Vector2(x, vector2.y);
        }
    }
}