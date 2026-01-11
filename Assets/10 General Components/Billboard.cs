using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;
    void Start()
    {
        _camera = Camera.main;
    }
    
    void LateUpdate()
    {
        
        var camForward = _camera.transform.forward;
        
        // no vertical offset, so set y to zero
        camForward.y = 0f;
        
        transform.forward = camForward;

        // // Previous code, just uncomment again if you dont like the new suggestion :)
        
        // Vector3 cameraPosition = _camera.transform.position;
        // cameraPosition.y = transform.position.y;
        // transform.LookAt(cameraPosition);
        // transform.Rotate(0f, 180f, 0f);
    }
}
