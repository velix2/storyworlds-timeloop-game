using UnityEngine;

public class GroundData : MonoBehaviour
{
    public enum groundType
    {
        DEFAULT,
        WOOD,
        WET,
        SNOW,
        TILE
    }

    public groundType type;
}