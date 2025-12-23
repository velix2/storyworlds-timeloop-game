using TimeManagement;
using TMPro;
using UnityEngine;

public class DebugTimeDisplay : MonoBehaviour
{
    private TMP_Text t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        t = GetComponent<TMP_Text>();
        TimeHandler.Instance.onTimePassed.AddListener(TimePassed);
        t.text = TimeHandler.Instance.CurrentTime.ToString();
    }

    private void TimePassed(TimePassedEventPayload arg0)
    {
        t.text = arg0.NewDaytimeInMinutes.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
