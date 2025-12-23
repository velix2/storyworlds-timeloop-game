using NPCs.NpcData.NpcRoutine;
using UnityEngine;

namespace NPCs.NpcData
{
    [CreateAssetMenu(fileName = "NPC Data", menuName = "Scriptable Objects/NpcData")]
    public class NpcData : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private string characterName;
        [SerializeField] private DayRoutine defaultRoutine;

        public GameObject Prefab => prefab;

        public string CharacterName => characterName;

        public DayRoutine DefaultRoutine => defaultRoutine;
    }
}