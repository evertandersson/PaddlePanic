using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatRotator : MonoBehaviour
{
    private float displayPosZ;
    private float hiddenPosZ;

    private void Start()
    {
        displayPosZ = transform.position.z;
        hiddenPosZ = transform.position.z - 25;
        transform.position = new Vector3 (transform.position.x, transform.position.y, hiddenPosZ);
    }

    private void OnEnable()
    {
        MainMenuButtons.onWorkShopClicked += DisplayBoats;
        MainMenuButtons.onWorkShopClosed += HideBoats;
    }
    private void OnDisable()
    {
        MainMenuButtons.onWorkShopClicked -= DisplayBoats;
        MainMenuButtons.onWorkShopClosed -= HideBoats;
    }

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 1, 0));
    }

    void DisplayBoats()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, displayPosZ);
    }
    void HideBoats()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, hiddenPosZ);
    }
}
