using UnityEngine;

public class BeginRace : MonoBehaviour
{
    private void Start()
    {
        EventManager.Raise(EventKey.RACE_START, new Void());
    }
}
