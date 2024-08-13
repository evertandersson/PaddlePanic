using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnableRenderFeature : MonoBehaviour
{
    [SerializeField] private List<ScriptableRendererFeature> features = new ();

    public void SetActive(bool value)
    {
        foreach (var feature in features)
        {
            feature.SetActive(value);
        }
    }
}
