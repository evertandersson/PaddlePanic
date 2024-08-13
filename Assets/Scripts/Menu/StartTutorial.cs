using System.Collections;
using UnityEngine;

public class StartTutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private PlayerAnimator playerAnimator;
    private PaddleButton paddleButton;

    private void Start()
    {
        playerAnimator = FindObjectOfType<PlayerAnimator>();
    }
    private IEnumerator Tutorial()
    {
        tutorialCanvas.SetActive(true);
        Time.timeScale = 0f;
        playerAnimator.isTutorial = true;
        yield return new WaitForSecondsRealtime(waitTime);
        Time.timeScale = 1;
        playerAnimator.isTutorial = false;
        gameObject.SetActive(false);
        tutorialCanvas.SetActive(false);
        
    }
    public float waitTime = 3f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameObject.name == "SharpTurnTutorial")
        {

            StartCoroutine(Tutorial());
            PaddleButton.EnableBrakes();
        }
        if (other.CompareTag("Player") && gameObject.name == "StartingTutorial")
        {

            StartCoroutine(Tutorial());
            PaddleButton.DisableBrakes();
        }
        else if (other.CompareTag("Player"))
        {
            StartCoroutine(Tutorial());

        }


    }

}