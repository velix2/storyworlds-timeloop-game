using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CollisionReporter : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;
    
    
    public UnityEvent<Collision> CollisionEnter = new();
    public UnityEvent<Collision> CollisionExit = new();

    private void OnCollisionEnter(Collision other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;
        CollisionEnter.Invoke(other);
    }

    private void OnCollisionExit(Collision other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;
        CollisionExit.Invoke(other);
    }

    public UnityEvent<Collider> TriggerEnter = new();
    public UnityEvent<Collider> TriggerExit = new();
    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;
        TriggerEnter.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;
        TriggerExit.Invoke(other);
    }
}
