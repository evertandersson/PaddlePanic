using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestRace : MonoBehaviour
{
    [SerializeField] private InputAction startRace;
    [SerializeField] private InputAction endRace;

    private void OnEnable()
    {
        startRace.Enable();
        endRace.Enable();
    }

    private void OnDisable()
    {
        startRace.Disable();
        endRace.Disable();
    }

    private void Update()
    {
        if (startRace.WasReleasedThisFrame())
        {
            EventManager.Raise(EventKey.RACE_START, new Void());
            Debug.Log("Race started");
            
            EventManager.Raise(EventKey.FOG_FADE_IN, new Void());

        }
        else if (endRace.WasReleasedThisFrame())
        {
            EventManager.Raise(EventKey.RACE_END, new Void());
            Debug.Log("Race ended");

            var results = ServiceLocator.GetService<RaceResults>();
            RaceData data = results.GetResults();
            
            
            EventManager.Raise(EventKey.FOG_FADE_OUT, new Void());
        }
    }
}
