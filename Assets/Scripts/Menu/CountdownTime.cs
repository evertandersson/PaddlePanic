using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    private TextMeshProUGUI countdownText;
    [SerializeField] private AudioSource countDownSource;

    private IEnumerator StartCountdown()
    {
        Time.timeScale = 0;
        countDownSource.Play();
        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1);
        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1);
        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1);
        countdownText.text = "START!";
        yield return new WaitForSecondsRealtime(1);
        Destroy(gameObject);
        Time.timeScale = 1;
    }
    private void Start()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(StartCountdown());
    }
}
