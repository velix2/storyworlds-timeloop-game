using System;
using System.Collections.Generic;
using NPCs.NpcData.NpcRoutine;
using SceneHandling;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private List<NpcData.NpcData> npcsToManage;

        private Dictionary<NpcModel, SceneMetaData> _npcModels;

        private List<NpcCharacter.NpcCharacter> _npcsInCurrentSceneVisually;
        
        public UnityEvent<NpcModel, SceneMetaData, NpcItinerary> onNpcSwitchedScene = new();

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
            // Instantiate data model for all NPCs
            _npcModels = new List<NpcModel>(npcsToManage.Count);

            foreach (var npcData in npcsToManage)
            {
                _npcModels.Add(new NpcModel(npcData.DefaultRoutine, this));
            }
        }

        private void Update()
        {
            foreach (var npcModel in _npcModels)
            {
                npcModel.Update(Time.deltaTime);
            }
        }

        private void OnSceneLoaded(string sceneName)
        {
        }

        public void SubmitNpcItinerary(NpcModel npcModel, List<Tuple<SceneMetaData, float>> pathWithTime)
        {
        }
    }
}