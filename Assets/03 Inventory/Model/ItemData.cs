using UnityEngine;

[CreateAssetMenu(menuName = "Resource/ItemData", fileName = "New ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    public string ItemName => itemName;

    [SerializeField] private string observationText;
    public string ObservationText => observationText;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
}
