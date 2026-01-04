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
        [SerializeField] private AnimatorOverrideController walkAnimator;
        [SerializeField] private DayRoutine defaultRoutine;
        private DayRoutine _currentRoutine;

        public DayRoutine CurrentRoutine
        {
            get
            {
                if (!_currentRoutine) _currentRoutine = DefaultRoutine;
                return _currentRoutine;
            }
            set => _currentRoutine = value;
        }

        public GameObject Prefab => prefab;

        public string CharacterName => characterName;

        public DayRoutine DefaultRoutine => defaultRoutine;
    }
}