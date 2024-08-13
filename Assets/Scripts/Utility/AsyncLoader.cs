using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoader
{
    public async void Load(List<string> names)
    {
        List<AsyncOperation> asyncOps = new List<AsyncOperation>();
        
        for (int i = 0; i < names.Count; i++)
        {
            asyncOps.Add(SceneManager.LoadSceneAsync(names[i], LoadSceneMode.Additive));
            asyncOps[i].allowSceneActivation = false;
        
            while (asyncOps[i].progress < 0.9f)
            {
                await Task.Yield();
            }
        }

        for (int i = 0; i < asyncOps.Count; i++)
        {
            asyncOps[i].allowSceneActivation = true;
        }
        
        var asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        
        while (!asyncOp.isDone)
        {
            await Task.Yield();
        }
    }
}
