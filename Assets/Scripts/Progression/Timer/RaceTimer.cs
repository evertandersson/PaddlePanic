using System;
using UnityEngine;

public class RaceTimer : MonoBehaviour
{
    private Timer timer = new Timer();
    
    public int Raw => (int)timer.elapsed;
    public string Formatted => GetFormatted();

    private void OnEnable()
    {
        EventManager.AddListener<Void>(EventKey.RACE_START, StartTimer);
        EventManager.AddListener<Void>(EventKey.RACE_END, StopTimer);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<Void>(EventKey.RACE_START, StartTimer);
        EventManager.RemoveListener<Void>(EventKey.RACE_END, StopTimer);
    }

    private void StartTimer(Void empty)
    {
        timer.elapsed = 0;
        timer.running = true;
    }

    private void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    private void StopTimer(Void empty)
    {
        timer.running = false;
    }
    
    /// <returns>Time as a string formatted as "mm:ss". Max value 59 minutes 59 seconds.</returns>
    private string GetFormatted()
    {
        return $"{Mathf.Min(timer.elapsed / 60, 59):00}:{timer.elapsed % 60:00}";
    }
}
