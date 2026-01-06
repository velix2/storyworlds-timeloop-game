using TimeManagement;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void AdvanceTime()
    {
        TimeHandler.PassTime(60);
    }
}