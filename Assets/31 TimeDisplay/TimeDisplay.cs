using System.Collections;
using TimeManagement;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField] private Transform minuteHandleTransform, hourHandleTransform;
    [SerializeField] private TMP_Text digitalText;
    
    [SerializeField] private float animationDuration = 3.0f;
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _animationCoroutine;
    private int _minutesDisplayed;

    private void OnEnable() => TimeHandler.Instance.onTimePassed.AddListener(OnTimePassed);
    private void OnDisable() => TimeHandler.Instance.onTimePassed.RemoveListener(OnTimePassed);

    private void Start()
    {
        _minutesDisplayed = TimeHandler.Instance.CurrentTime;
        SetDisplays(_minutesDisplayed);
    }

    private void OnTimePassed(TimePassedEventPayload payload)
    {
        if (payload.DayHasEnded)
        {
            _minutesDisplayed = payload.NewDaytimeInMinutes;
            SetDisplays(_minutesDisplayed);
            return;
        }

        if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(AnimateTime(payload.NewDaytimeInMinutes));
    }

    private IEnumerator AnimateTime(int targetMinutes)
    {
        float elapsed = 0;
        int startMinutes = _minutesDisplayed;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Use the curve for Ease In/Out
            float easedT = easingCurve.Evaluate(t);
            
            _minutesDisplayed = (int)Mathf.Lerp(startMinutes, targetMinutes, easedT);
            SetDisplays(_minutesDisplayed);
            
            yield return null;
        }

        _minutesDisplayed = targetMinutes;
        SetDisplays(_minutesDisplayed);
    }

    private void SetDisplays(int minutes)
    {
        digitalText.text = TimeHandler.GameMinutesToFormatted(minutes);
        
        float hours = (minutes / 60f) % 12;
        float mins = minutes % 60;
        
        hourHandleTransform.rotation = Quaternion.Euler(0, 0, -hours * 30f);
        minuteHandleTransform.rotation = Quaternion.Euler(0, 0, -mins * 6f);
    }
}