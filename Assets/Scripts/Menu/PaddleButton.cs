using UnityEngine;
using UnityEngine.EventSystems;



public class PaddleButton : MonoBehaviour
{

    private PlayerController playerController;
    private static GameObject leftBrake;
    private static GameObject rightBrake;
    
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }



    public void OnLeftPaddlePressed(BaseEventData data)
    {
        playerController.leftButton = 1;
    }
    public void OnLeftPaddleReleased(BaseEventData data)
    {
        playerController.leftButton = 0;
    }

    public void OnLeftBrakePressed(BaseEventData data)
    {
        playerController.leftButton = -1;
    }
    public void OnLeftBrakeReleased(BaseEventData data)
    {
        playerController.leftButton = 0;
    }
    public void OnRightPaddlePressed(BaseEventData data)
    {
        playerController.rightButton = 1;
    }    
    public void OnRightPaddleReleased(BaseEventData data)
    {
        playerController.rightButton = 0;
    }
    public void OnRightBrakePressed(BaseEventData data)
    {
        playerController.rightButton = -1;
    }    
    public void OnRightBrakeReleased(BaseEventData data)
    {
        playerController.rightButton = 0;
    }
    public static void DisableBrakes()
    {
        leftBrake = GameObject.Find("LeftBrake"); 
        rightBrake = GameObject.Find("RightBrake"); 
        leftBrake.SetActive(false); 
        rightBrake.SetActive(false);

    }
    public static void EnableBrakes()
    {
        leftBrake.SetActive(true);
        rightBrake.SetActive(true);
    }
}
