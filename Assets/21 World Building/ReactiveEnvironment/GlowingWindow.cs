using System;
using TimeManagement;
using UnityEngine;

namespace World_Building.ReactiveEnvironment
{
    [RequireComponent(typeof(Renderer))]
    public class GlowingWindow : MonoBehaviour
    {
        [SerializeField] private int enableTimeMinutes, disableTimeMinutes;
        [SerializeField] private Material glowMaterial;
        private Material _initialMaterial;

        [SerializeField] private string materialNameToReplace;
        private int _materialIndexToReplace = -1;
        
        private Renderer _renderer;

        private void Awake()
        {
            _renderer =  GetComponent<Renderer>();
        }

        private void Start()
        {
            // Subscribe to Time Event
            TimeHandler.Instance.onTimePassed.AddListener(OnTimePasssed);
            
            // Find the index of the material that matches the specified name
            for (var i = 0; i < _renderer.materials.Length; i++)
            {
                if (_renderer.materials[i].name != materialNameToReplace) continue;
                _materialIndexToReplace = i;
                break;
            }

            if (_materialIndexToReplace == -1)
            {
                Debug.LogError($"Material {materialNameToReplace} not found");
                return;
            }
            
            _initialMaterial =  _renderer.materials[_materialIndexToReplace];
        }

        private void OnTimePasssed(TimePassedEventPayload payload)
        {
            var t = payload.NewDaytimeInMinutes;
            
            bool setLightIsOn = false;
            
            // Case enabled time span wraps around midnight (off < on)
            if (disableTimeMinutes < enableTimeMinutes)
            {
                // light is on when its before disable or after enable
                setLightIsOn = t < disableTimeMinutes ||  t > enableTimeMinutes;
            }
            // Case light turns on and off within a day
            else
            {
                setLightIsOn = enableTimeMinutes <= t && t < disableTimeMinutes;
            }
            
            if (setLightIsOn) OverrideMaterial();
            else RestoreMaterial();
        }

        private void OverrideMaterial()
        {
            _renderer.materials[_materialIndexToReplace] = glowMaterial;
        }

        private void RestoreMaterial()
        {
            _renderer.materials[_materialIndexToReplace] = _initialMaterial;
        }
    }
}