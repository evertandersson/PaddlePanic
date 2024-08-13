using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct Star
{
    public int score;
}

[Serializable]
public struct PerfectTime
{
    public int minutes;
    public int seconds;
    public int score;
    
    public readonly int Raw => minutes * seconds;
}

public struct RaceData
{
    public string time;
    public int score;
    public int stars;
}

public class RaceResults : MonoBehaviour
{
    [SerializeField] private RaceTimer timer;
    [SerializeField] private CheckpointScore checkpointScore;
    
    
    [SerializeField] private List<Star> starTiers;
    [SerializeField] private int baseScore = 1000;
    [SerializeField] private PerfectTime perfectTime;
    [SerializeField] private int scoreLostPerSec;

    private void OnEnable()
    {
        ServiceLocator.Register(this);
    }

    private void OnDisable()
    {
        ServiceLocator.Deregister<RaceResults>();
    }

    private void Start()
    {
        EventManager.Raise(EventKey.RACE_START, new Void());
    }

    public static RaceData GetMockResults()
    {
        RaceData raceData;
        
        raceData.time = $"0{Random.Range(0, 10)}:{Random.Range(10, 60)}";
        raceData.score = Random.Range(1000, 3001);
        raceData.stars = Random.Range(0, 4);
        
        Debug.Log($"Time: {raceData.time}");
        Debug.Log($"Score: {raceData.score}");
        Debug.Log($"Stars: {raceData.stars}");

        return raceData;
    }
    
    public RaceData GetResults()
    {
        RaceData raceData;

        raceData.time = GetTime();
        raceData.score = GetScore();
        raceData.stars = GetStars(raceData.score);
        
        Debug.Log($"Time: {raceData.time}");
        Debug.Log($"Score: {raceData.score}");
        Debug.Log($"Stars: {raceData.stars}");

        
        return raceData;
    }

    private string GetTime()
    {
        return timer.Formatted;
    }

    private int GetScore()
    {
        int timeDelta = Mathf.Min(timer.Raw - perfectTime.Raw, 0);
        int timeScore = perfectTime.score - (timeDelta * scoreLostPerSec);
        int totalScore = baseScore + checkpointScore.GetScore() + timeScore;

        return totalScore;
    }

    private int GetStars(int score)
    {
        for (int i = starTiers.Count; i > -1; i--)
        {
            if (score >= starTiers[i-1].score)
                return i;
        }

        return 0;
    }
}
