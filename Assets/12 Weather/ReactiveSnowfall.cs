using System;
using TimeManagement;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ReactiveSnowfall : DaytimePhaseChangeSubscriber<float>
{
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    protected override void ApplyElement(float snowflakeMultiplier, DaytimePhase _)
    {
        var emission = _particleSystem.emission;
        emission.rateOverTimeMultiplier = snowflakeMultiplier;
    }
}
