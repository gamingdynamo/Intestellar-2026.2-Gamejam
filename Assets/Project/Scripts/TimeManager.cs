using TMPro;
using UnityEngine;
using NaughtyAttributes;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float secondsPassed = 0;
    [SerializeField] private float timeMultiplier = 60;
    [SerializeField] private TMP_Text timeText = null;

    void Update()
    {
        this.secondsPassed += Time.deltaTime;
        
        if (timeText != null)
        {
            timeText.text = getTimeText();
        }
    }

    [Button("Skip to next day")]
    void Sleep()
    {
        int days = getDay() + 1;
        int hours = (int)7;
        this.secondsPassed = ( ( (float)days * 24.0f * 60.0f * 60.0f ) + ( (float)hours * 60.0f * 60.0f ) ) / (float)timeMultiplier;
    }

    public int getDay()
    {
        return (int)((timeMultiplier * secondsPassed) / 86400);
    }

    string getTimeText()
    {
        // Calculate days, hours, minutes, and seconds
        float multipliedTime = timeMultiplier * secondsPassed;
        int days = getDay();
        int hours = (int)((multipliedTime % 86400) / 3600);
        int minutes = (int)((multipliedTime % 3600) / 60);
        int seconds = (int)(multipliedTime % 60);

        // Formats as HH:MM and Day on a new line
        return string.Format("{0:00}:{1:00}\nDay: {3}", hours, minutes, seconds, days);
    }
}