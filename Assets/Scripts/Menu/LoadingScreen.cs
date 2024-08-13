using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    private readonly AsyncLoader loader = new ();

    private void Awake()
    {
        loader.Load(LoadingList.Get());
    }
}

public static class LoadingList
{
    private static List<string> scenesToLoad;
    
    public static void Set(List<string> names)
    {
        scenesToLoad = names;
    }

    public static List<string> Get()
    {
        return scenesToLoad;
    }
}
