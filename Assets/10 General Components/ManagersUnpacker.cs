using System;
using UnityEngine;

/// <summary>
/// Runs before Awake and unpacks all children. Helps to bundle Singletons into a single prefab.
/// </summary>
public class ManagersUnpacker : MonoBehaviour
{
    private void Awake()
    {
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
