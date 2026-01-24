using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    public static CinemachineCamera mainVirtual { get; private set; }
    private static CinemachineCamera currentVCam;
    private static CinemachineBrain brain;

    private static CinemachineBlendDefinition cutBlend = new CinemachineBlendDefinition(
        CinemachineBlendDefinition.Styles.Cut,
        0f);

    private static CinemachineBlendDefinition easeIn = new CinemachineBlendDefinition(
        CinemachineBlendDefinition.Styles.EaseIn, 1.0f);

    [Serializable]
    public enum BlendType
    {
        DEFAULT,
        CUT
    }

    private static BlendType currentBlend;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.Log("More than one CameraManager found...");
        }

        instance = this;
        brain = instance.GetComponent<CinemachineBrain>();
    }

    private void Start()
    {
        Invoke(nameof(GetMainCamera), 1.0f);
    }
    
    private void GetMainCamera()
    {
        mainVirtual = brain.ActiveVirtualCamera as CinemachineCamera;
        print("Brain: " + brain.ActiveVirtualCamera);
        print("MainCam: " +mainVirtual);
    }

    public static void FocusCam(CinemachineCamera cam, BlendType blend = BlendType.DEFAULT)
    {
        SetBlend(blend);
        if (mainVirtual) mainVirtual.enabled = false;
        if (currentVCam) currentVCam.enabled = false;
        currentVCam = cam;
        currentVCam.enabled = true;
    }

    public static void ChangeMainCamera(CinemachineCamera cam, BlendType blend = BlendType.DEFAULT)
    {
        SetBlend(blend);
        if (mainVirtual) mainVirtual.enabled = false;
        mainVirtual = cam;
        mainVirtual.enabled = true;
        Debug.Log("CHanged main cam to: " + mainVirtual);
    }
    
    public static void BackToMain(BlendType blend = BlendType.DEFAULT)
    {
        SetBlend(blend);
        if (currentVCam != null)
        {
            currentVCam.enabled = false;
            currentVCam = null;
        }
        
        CinemachineCamera cam = brain.ActiveVirtualCamera as CinemachineCamera;
        if (cam != null && cam != mainVirtual)
        {
            cam.enabled = true;
        }

        mainVirtual.enabled = true;
    }
    
    private static void SetBlend(BlendType blend)
    {
        if (currentBlend == blend) return;
        switch (blend)
        {
            case BlendType.CUT:
                brain.DefaultBlend = cutBlend;
                break;
            case BlendType.DEFAULT:
                brain.DefaultBlend = easeIn;
                break;
            default:
                Debug.Log("Blend setting for \"" + blend + "\" was not defined yet." );
                break;
        }
        currentBlend = blend;
        print(brain.DefaultBlend.Style);
    }


}
