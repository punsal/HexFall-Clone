using System;
using UnityEngine;

namespace Utility.Behaviour.Camera.Data
{
    [Serializable]
    public struct CameraTransformStationData
    {
        public string stationName;
        public Transform station;
        public float flightDuration;
        public float stayDuration;
    }
}