using System;
using UnityEngine;
using Utility.Behaviour.Camera.Type;

namespace Utility.Behaviour.Camera.Data
{
    [Serializable]
    public struct CameraTransformData
    {
        public string transformName;
        public Transform anchorTransform;
        public float flightDuration;
        public CameraTransformDataType type;
        public float returnDuration;
        public CameraTransformStationData[] stationData;
    }
}