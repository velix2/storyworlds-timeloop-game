using System;
using System.Collections.Generic;
using System.Linq;
using NPCs.NpcData;
using NPCs.NpcData.NpcRoutine;
using SceneHandling;
using TimeManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NPCs
{
    /// <summary>
    /// This singleton manages references to all NPCs in the current scene, as well as all NPCs globally.
    /// </summary>
    public class NpcHandler : MonoBehaviour
    {
        /// <summary>
        /// List of all NPCs that are in the game world. Currently a list of NPC ids.
        /// </summary>
        [SerializeField] private List<NpcModel> npcsToManage;

        private List<NpcCharacter.NpcCharacter> _npcViewsInCurrentScene;
        
        #region Singleton

        // Singleton stuff

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static NpcHandler Instance;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        private void Start()
        {
            // Subscribe to Time passed event
            TimeHandler.Instance.onTimePassed.AddListener(OnTimePassed);
        }

        private void OnTimePassed(TimePassedEventPayload payload)
        {
            UpdateNPCs(payload.NewDaytimeInMinutes);
        }

        private void UpdateNPCs(int daytime)
        {
            // Query all Npc Models that should be in the currently active scene according to their itinerary
            var newNpcModelsInCurrentScene = npcsToManage.Where(model =>
                model.CurrentRoutine.GetCurrentRoutineElement(daytime).TargetScene
                    .RepresentedScene == SceneManager.GetActiveScene()).ToList();
            
            // Figure out all NPCs that now need to leave, i.e. where their View object still exists, but their model is no longer part of the scene
            var npcViewsToLeave = _npcViewsInCurrentScene
                .Where(character => !newNpcModelsInCurrentScene.Contains(character.Model)).ToList();

            // TODO update their state to leaving

            // Figure out all NPCs that are new and need a new view element created
            var npcModelsToSpawn = newNpcModelsInCurrentScene
                .Where(model => _npcViewsInCurrentScene.TrueForAll(character => character.Model != model)).ToList();

            // TODO spawn these models and append them to viewsList
            
        }
        
        private void OnSceneLoaded()
        {
            // Clear views list so we start fresh
            _npcViewsInCurrentScene.Clear();
            
            UpdateNPCs(TimeHandler.Instance.CurrentTime);
        }
    }
}