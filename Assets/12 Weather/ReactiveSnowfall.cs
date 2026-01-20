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
        var shouldSnow = StateTracker.IntroState >= StateTracker.IntroStates.SonCallCompleted;
           
        // No snow on intro first day
        if (!shouldSnow) _particleSystem.Stop();
        // Resume snow else
        else if (_particleSystem.isStopped) _particleSystem.Play();
        
        var emission = _particleSystem.emission;
        emission.rateOverTimeMultiplier = snowflakeMultiplier;
    }
}
