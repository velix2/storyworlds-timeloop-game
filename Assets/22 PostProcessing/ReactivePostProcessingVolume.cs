using System;
using TimeManagement;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class ReactivePostProcessingVolume : DaytimePhaseChangeSubscriber<VolumeProfile>
{
    private Volume _volume;

    private void Awake()
    {
        _volume = GetComponent<Volume>();
    }

    protected override void ApplyElement(VolumeProfile elementToApply, DaytimePhase _)
    {
        _volume.profile = elementToApply;
    }
}
