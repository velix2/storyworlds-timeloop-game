using System;
using NPCs.NpcData.NpcRoutine;
using TimeManagement;
using UnityEngine;

namespace NPCs.NpcData
{
    [CreateAssetMenu(fileName = "NPC Model", menuName = "Scriptable Objects/NpcModel")]
    public class NpcModel : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private string characterName;
        [SerializeField] private DayRoutine defaultRoutine;
        public DayRoutine CurrentRoutine { get; set; }

        public GameObject Prefab => prefab;

        public string CharacterName => characterName;

        public DayRoutine DefaultRoutine => defaultRoutine;

        private void Awake()
        {
            CurrentRoutine = DefaultRoutine;
        }
    }
}