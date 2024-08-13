using UnityEngine;

public static class ProjectSettings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void SetTargetFramerate()
    {
        Application.targetFrameRate = 60;
    }
}
