using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PauseMenu;

public class AssignVariables : MonoBehaviour
{
    private EventTrigger leftPaddle, leftBrake, rightPaddle, rightBrake;



    private StaminaScript staminaScript = null;
    PaddleButton paddleButton = null;

    private bool initialized;
    void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        onMenuClicked += DestroyPlayer;
    }

    private void OnDisable()
    {
        onMenuClicked -= DestroyPlayer;
    }

    private void Start()
    {
        Initialize();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameObject.SetActive(true);
    }

    public void Initialize() {
        if(initialized) 
            return;

        paddleButton = GetComponent<PaddleButton>();
        staminaScript = GetComponent<StaminaScript>();

        Transform gameCanvas = GameObject.Find("GameCanvas").transform;
        //Debug.Log(gameCanvas);

        leftPaddle = gameCanvas.Find("LeftPaddle").gameObject.AddComponent<EventTrigger>();
        leftBrake = gameCanvas.Find("LeftBrake").gameObject.AddComponent<EventTrigger>();
        rightPaddle = gameCanvas.Find("RightPaddle").gameObject.AddComponent<EventTrigger>();
        rightBrake = gameCanvas.Find("RightBrake").gameObject.AddComponent<EventTrigger>();

        if (leftPaddle) {
            EventTrigger.Entry pressed = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            EventTrigger.Entry released = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            pressed.callback.AddListener(paddleButton.OnLeftPaddlePressed);
            released.callback.AddListener(paddleButton.OnLeftPaddleReleased);
            leftPaddle.triggers.Add(pressed);
            leftPaddle.triggers.Add(released);
        }
        if (leftBrake) {
            EventTrigger.Entry pressed = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            EventTrigger.Entry released = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            pressed.callback.AddListener(paddleButton.OnLeftBrakePressed);
            released.callback.AddListener(paddleButton.OnLeftBrakeReleased);
            leftBrake.triggers.Add(pressed);
            leftBrake.triggers.Add(released);
        }
        if (rightPaddle) {
            EventTrigger.Entry pressed = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            EventTrigger.Entry released = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            pressed.callback.AddListener(paddleButton.OnRightPaddlePressed);
            released.callback.AddListener(paddleButton.OnRightPaddleReleased);
            rightPaddle.triggers.Add(pressed);
            rightPaddle.triggers.Add(released);
        }
        if (rightBrake) {
            EventTrigger.Entry pressed = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            EventTrigger.Entry released = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            pressed.callback.AddListener(paddleButton.OnRightBrakePressed);
            released.callback.AddListener(paddleButton.OnRightBrakeReleased);
            rightBrake.triggers.Add(pressed);
            rightBrake.triggers.Add(released);
        }

        Transform staminaBar = gameCanvas.Find("Stamina_Bar");
        Image staminaProgressBar = staminaBar.Find("Stamina").GetComponent<Image>();
        
        Image staminaBG = staminaBar.Find("StaminaBG").GetComponent<Image>();

        staminaScript.AssignStaminaUI(staminaProgressBar, staminaBG);

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.AssignTarget(transform);
        cameraFollow.Initialize();

        initialized = true;
    }

    private void DestroyPlayer()
    {
        Debug.Log("Destroy player");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
    }
}
