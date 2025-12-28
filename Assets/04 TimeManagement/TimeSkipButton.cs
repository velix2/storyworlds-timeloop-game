using TimeManagement;
using UnityEngine;

public class TimeSkipButton : MonoBehaviour
{
    public void CallPassTime(int min)
    {
        TimeHandler.PassTime(min);
    }

    public void CallSkipToNextDayphase()
    {
        TimeHandler.SkipToNextDaytimePhase();
    }
}
