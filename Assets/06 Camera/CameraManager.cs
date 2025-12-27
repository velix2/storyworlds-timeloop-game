using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    private static CinemachineCamera mainVirtual;
    private static CinemachineCamera current;
    private static CinemachineBrain brain;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.Log("More than one CameraManager found...");
        }

        instance = this;
        mainVirtual = GameObject.FindGameObjectWithTag("DefaultVirtualCam").GetComponent<CinemachineCamera>();
    }

    public static void FocusCam(CinemachineCamera cam)
    {
        mainVirtual.enabled = false;
        if (current) current.enabled = false;
        current = cam;
        current.enabled = true;
    }

    public static void BackToMain()
    {
        if (current)
        {
            current.enabled = false;
            current = null;
        }

        mainVirtual.enabled = true;
    }

    public static void ChangeMainCamera(CinemachineCamera cam)
    {
        mainVirtual.enabled = false;
        mainVirtual = cam;
        mainVirtual.enabled = true;
    }
    
}
