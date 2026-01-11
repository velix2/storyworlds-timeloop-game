using TimeManagement;
using UnityEngine;

public class ReactiveLightManager : DaytimePhaseChangeSubscriber<GameObject>
{
    protected override void ApplyElement(GameObject elementToApply, DaytimePhase _)
    {
        // Disable all lighting
        elementMorning.SetActive(false);
        elementAfternoon.SetActive(false);
        elementEvening.SetActive(false);
        elementNight.SetActive(false);
        
        // Reenable current one
        elementToApply.SetActive(true);
        
    }
}
