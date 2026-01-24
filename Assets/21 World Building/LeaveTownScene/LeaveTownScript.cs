using System;
using System.Collections;
using System.Linq;
using Meryuhi.Rendering;
using TimeManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class LeaveTownScript : MonoBehaviour
{
    [SerializeField] private VolumeProfile ppVolumeProfile;
    private FullScreenFog _fullScreenFogComponent;
    
    [SerializeField] private TextAsset dialogueJson;

    [SerializeField] private float animationDuration;

    [SerializeField] private float targetFogDensity;
    [SerializeField] private float targetFogIntensity;

    private float _fogDistanceDecreasePerSecond;
    private float _fogDensityIncreasePerSecond;
    private float _fogIntensityIncreasePerSecond;

    private float _animationProgressSec;
    
    private float _initDensity;
    private float _initIntensity;
    private float _initDistance;


    private void Start()
    {
        _fullScreenFogComponent = (FullScreenFog) ppVolumeProfile.components.First(component => component is FullScreenFog);

        if (!_fullScreenFogComponent)
        {
            Debug.LogError("Could not find FullScreenFog component on Volume");
            return;
        }

        // Setup rates
        _initDensity = _fullScreenFogComponent.density.value;
        _fogDensityIncreasePerSecond = (targetFogDensity - _initDensity) / animationDuration;
        
        _initIntensity = _fullScreenFogComponent.intensity.value;
        _fogIntensityIncreasePerSecond = (targetFogIntensity - _initIntensity) / animationDuration;
        
        _initDistance = _fullScreenFogComponent.startLine.value;
        _fogDistanceDecreasePerSecond = _initDistance / animationDuration;

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // Start the Dialogue
        DialogueManager.Instance.EnterDialogueMode(dialogueJson);
        
        // Await dialogue end
        yield return new WaitWhile(() => DialogueManager.Instance.DialogueIsPlaying);
        
        // Animate fog stuff
        while (_animationProgressSec <= animationDuration)
        {
            _fullScreenFogComponent.density.value += _fogDensityIncreasePerSecond * Time.deltaTime;
            _fullScreenFogComponent.intensity.value += _fogIntensityIncreasePerSecond * Time.deltaTime;
            
            _fullScreenFogComponent.startLine.value -= _fogDistanceDecreasePerSecond * Time.deltaTime;

            _animationProgressSec += Time.deltaTime;
            yield return null;
        }

        StateTracker.EvelynQuestState = StateTracker.EvelynQuestStates.TriedLeavingTown;
        
        TimeHandler.ResetDay();
        
        print("is this visible");
    }

    private void OnDisable()
    {
        // revert settings
        _fullScreenFogComponent.density.value = _initDensity;
        _fullScreenFogComponent.intensity.value = _initIntensity;
        _fullScreenFogComponent.startLine.value = _initDistance;
    }
}
