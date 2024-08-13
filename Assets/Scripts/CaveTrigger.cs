using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CaveTrigger : MonoBehaviour
{
    [Serializable]
    public enum TriggerType
    {
        ENTRANCE,
        EXIT,
    }

    [SerializeField] private TriggerType triggerType;
    
    [SerializeField] private float fogEnd = 40f;
    
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private float fadeFogTarget = 100f;

    [SerializeField] private float ambientIntensity;
    [SerializeField] private float reflectionIntensity;
    [SerializeField] private float lightFadeDurationEnter;
    [SerializeField] private float lightFadeDurationExit;
    [SerializeField] private float playerLightFadeDuration;
    [SerializeField] private float playerLightIntensity;



    private float ambientIntensityOriginal;
    private float reflectionIntensityOriginal;

    
    private Color fogColor = Color.black;
    
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Material cutoutMaterial;

    private new Camera camera;
    private Light playerLight;

    private MeshRenderer meshRenderer;
    
    private static readonly int TintColor = Shader.PropertyToID("_Tint");
    private static readonly int DepthRange = Shader.PropertyToID("_DepthRange");
    private static readonly int Exposure = Shader.PropertyToID("_Exposure");


    private void Awake()
    {
        if (triggerType == TriggerType.ENTRANCE)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = cutoutMaterial;
            meshRenderer.material.SetFloat(DepthRange, fogEnd);
        }

        ambientIntensityOriginal = RenderSettings.ambientIntensity;
        reflectionIntensityOriginal = RenderSettings.reflectionIntensity;

        var go = GameObject.FindGameObjectWithTag("PlayerLight");
        if (go != null)
        {
            playerLight = go.GetComponent<Light>();
        }
        
        // Sergei: Would rather have a serialized field! Oh, the sacrifices we make for the sake of convenience...
        camera = Camera.main;
    }

    private void SetFog(bool active, Color color, CameraClearFlags cameraFlags)
    {
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogEndDistance = fogEnd;
        
        RenderSettings.fog = active;
        RenderSettings.fogColor = color;

        camera.clearFlags = cameraFlags;
        camera.backgroundColor = color;
        
        skyboxMaterial.SetFloat(Exposure, 0);
    }
    
    private IEnumerator FadeCoroutine(float duration, float target, Color skyboxTarget, CameraClearFlags cameraFlags)
    {
        float accumulator = 0;

        camera.clearFlags = cameraFlags;
        camera.backgroundColor = fogColor;
        
        while (accumulator < duration)
        {
            float t = accumulator / duration;
            
            var fogStartDistance = Mathf.SmoothStep(RenderSettings.fogStartDistance, target - 1, t);
            var fogEndDistance = Mathf.SmoothStep(RenderSettings.fogEndDistance, target, t);
            var skyboxExposure = Mathf.Lerp(0, 1, t);
            
            RenderSettings.fogStartDistance = fogStartDistance;
            RenderSettings.fogEndDistance = fogEndDistance;
            skyboxMaterial.SetFloat(Exposure, skyboxExposure);
            
            accumulator += Time.deltaTime;
            
            yield return null;
        }
        
        RenderSettings.fogStartDistance = target - 1;
        RenderSettings.fogEndDistance = target;
        RenderSettings.fog = false;
    }
    
    private IEnumerator AmbientCoroutine(float duration, float ambientTarget, float reflectionTarget)
    {
        float accumulator = 0;
        
        while (accumulator < duration)
        {
            float t = accumulator / duration;
            
            var _ambientIntensity = Mathf.Lerp(1, ambientTarget, t);
            var _reflectionIntensity = Mathf.Lerp(1, reflectionTarget, t);
            
            RenderSettings.ambientIntensity = _ambientIntensity;
            RenderSettings.reflectionIntensity = _reflectionIntensity;
            
            accumulator += Time.deltaTime;
            
            yield return null;
        }
        
        RenderSettings.ambientIntensity = ambientTarget;
        RenderSettings.reflectionIntensity = reflectionTarget;
    }
    
    private IEnumerator PlayerLightCoroutine(float duration, float start, float target)
    {
        float accumulator = 0;
        
        while (accumulator < duration)
        {
            float t = accumulator / duration;
            
            playerLight.intensity = Mathf.Lerp(start, target, t);
            
            accumulator += Time.deltaTime;
            
            yield return null;
        }

        playerLight.intensity = target;
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (triggerType)
        {
            case TriggerType.ENTRANCE:
                OnEntrance(other);
                break;
            case TriggerType.EXIT:
                OnExit(other);
                break;
        }
    }

    private void OnEntrance(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            SetFog(true, Color.black, CameraClearFlags.SolidColor);
            StartCoroutine(AmbientCoroutine(lightFadeDurationEnter, ambientIntensity, reflectionIntensity));

            if (playerLight != null)
            {
                StartCoroutine(PlayerLightCoroutine(playerLightFadeDuration, 0, playerLightIntensity));
            }
            
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
    }

    private void OnExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeCoroutine(fadeDuration, fadeFogTarget, new Color(0.5f, 0.5f, 0.5f, 0.5f), CameraClearFlags.Skybox));
            StartCoroutine(AmbientCoroutine(lightFadeDurationExit, ambientIntensityOriginal, reflectionIntensityOriginal));

            if (playerLight != null)
            {
                StartCoroutine(PlayerLightCoroutine(playerLightFadeDuration, playerLightIntensity, 0));
            }
        }
    }

    private void OnDestroy()
    {
        skyboxMaterial.SetFloat(Exposure, 1);
    }
}
