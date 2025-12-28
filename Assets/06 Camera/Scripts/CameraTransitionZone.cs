using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraTransitionZone : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CameraManager.BlendType blendType;
    [SerializeField] private CameraType cameraType;
    private enum CameraType
    {
        MAIN,
        FOCUS
    }
    
    
    
    private static bool firstBlend;

    private void Awake()
    {
        StartCoroutine(FirstBlendPeriod());
    }

    private IEnumerator FirstBlendPeriod()
    {
        firstBlend = true;
        yield return new WaitForEndOfFrame();
        firstBlend = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        
        switch (cameraType)
        {
            case CameraType.MAIN:
                if (firstBlend)
                {
                    CameraManager.ChangeMainCamera(cam, CameraManager.BlendType.CUT);
                }
                else
                {
                    CameraManager.ChangeMainCamera(cam, blendType);
                }
                break;
            case CameraType.FOCUS:
                CameraManager.FocusCam(cam, blendType);
                break;
            default:
                Debug.Log("Case for " + blendType + "has not been implemented yet.");
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        switch (cameraType)
        {
            case CameraType.FOCUS:
                CameraManager.BackToMain();
                break;
            case CameraType.MAIN:
                break;
            default:
                Debug.Log("Case for " + blendType + "has not been implemented yet.");
                break;
        }
    }
}
