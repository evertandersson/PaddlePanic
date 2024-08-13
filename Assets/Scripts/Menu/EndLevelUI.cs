using System;
using TMPro;
using UnityEngine;

public class EndLevelUI : MonoBehaviour
{
    private PauseMenu pauseMenu;
    
    [Header("Stars")]
    [SerializeField] private GameObject Star1;
    [SerializeField] private GameObject Star2;
    [SerializeField] private GameObject Star3;
    [Header("Text")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [Header("Canvas")]
    [SerializeField] private GameObject GameCanvas;
    [SerializeField] private GameObject EndCanvas;
    private RaceData raceData;

    [SerializeField] private AudioSource EndLevelSource;
    private void OnEnable()
    {
        GameCanvas = GameObject.Find("GameCanvas");
        pauseMenu = GetComponent<PauseMenu>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            EventManager.Raise(EventKey.RACE_END, new Void());
            if (EndLevelSource && !EndLevelSource.isPlaying)
            {
                EndLevelSource.Play();
            }
            
            ChangeUI(); //Switches from Game UI to End UI
            raceData = ServiceLocator.GetService<RaceResults>().GetResults(); // Sergei: Could cash it if you want.
            scoreText.text = "Score: "+raceData.score;
            timeText.text = "Time: "+raceData.time;
            ActivateStars(raceData.stars);
            
            SaveLoad.AddStars(raceData.stars);
            SaveLoad.Save();
            
            Time.timeScale = 0;
            pauseMenu.isPaused = true;
        }
    }

    private void ChangeUI()
    {
        EndCanvas.SetActive(true);
        Star1.SetActive(false);
        Star2.SetActive(false);
        Star3.SetActive(false);
        GameCanvas.SetActive(false);
    }
    private void ActivateStars(int stars)
    {
        if (stars == 3)
        {
            Star1.SetActive(true);
            Star2.SetActive(true);
            Star3.SetActive(true);
        }
        else if (stars == 2)
        {
            Star1.SetActive(true);
            Star2.SetActive(true);
        }
        else if (stars == 1)
        {
            Star1.SetActive(true);
        }
    }
}
