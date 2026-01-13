using System;
using Unity.Cinemachine;
using UnityEngine;

public class SceneCameraProvider : MonoBehaviour, ICameraProvider
{
    [Serializable]
    public struct CameraSlot
    {
        public string id;
        public CinemachineCamera vcam;
    }

    public CameraSlot[] cameras;

    public CinemachineCamera GetCamera(string id)
    {
        foreach (var c in cameras)
            if (c.id == id)
                return c.vcam;
        return null;
    }
}
