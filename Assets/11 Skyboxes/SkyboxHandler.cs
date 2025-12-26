using TimeManagement;
using UnityEngine;

public class SkyboxHandler : DaytimePhaseChangeSubscriber<Material>
{
    protected override void ApplyElement(Material elementToApply, DaytimePhase _)
    {
        // Apply the skybox
        RenderSettings.skybox = elementToApply;
    }
}