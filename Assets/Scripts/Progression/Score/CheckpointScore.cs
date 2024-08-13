using UnityEngine;

public class CheckpointScore : MonoBehaviour
{
    private int score;
    
    private void OnEnable()
    {
        EventManager.AddListener<Void>(EventKey.RACE_START, OnRaceStart);
        EventManager.AddListener<int>(EventKey.SCORE_GIVE, AddScore);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<Void>(EventKey.RACE_START, OnRaceStart);
        EventManager.RemoveListener<int>(EventKey.SCORE_GIVE, AddScore);
    }

    private void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"Score: {score}");
    }

    public int GetScore()
    {
        return score;
    }

    private void OnRaceStart(Void empty)
    {
        score = 0;
    }
}
