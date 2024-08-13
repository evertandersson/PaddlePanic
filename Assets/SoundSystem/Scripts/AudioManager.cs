using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private AudioMixer mixer;

    [Header("Main Menu Music")] [SerializeField]
    private AudioSource mainMenuSource;

    [Header("Background Music")] [SerializeField]
    private AudioSource backgroundSource;

    [Header("UIButtonSound")] [SerializeField]
    private AudioSource UIButtonSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeVolume();
        SetMenuButtons();
        mixer.SetFloat("MasterVolume", ToDecibels(PlayerPrefs.GetFloat("MasterVolume")));
        mixer.SetFloat("MusicVolume", ToDecibels(PlayerPrefs.GetFloat("MusicVolume")));
        mixer.SetFloat("SFXVolume", ToDecibels(PlayerPrefs.GetFloat("SFXVolume")));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeVolume();

        switch (scene.buildIndex)
        {
            case 0:
                SetMenuButtons();
                mainMenuSource.Play();
                backgroundSource.Stop();
                break;
            
            case 2:
                mainMenuSource.Stop();
                backgroundSource.Play();
                break;
            
            case 3:
                mainMenuSource.Stop();
                backgroundSource.Play();
                break;
            
            case 4:
                mainMenuSource.Stop();
                backgroundSource.Play();
                break;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void UIButtonSound()
    {
        UIButtonSource.Play();
    }

    public void SetMasterVolume(float volume)
    {
        mixer.SetFloat("MasterVolume", ToDecibels(volume));
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", ToDecibels(volume));
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", ToDecibels(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private void ChangeVolume()
    {
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var slider in sliders)
        {
            switch (slider.gameObject.tag)
            {
                case "VolumeSlider":
                    slider.value = PlayerPrefs.GetFloat("MasterVolume");
                    slider.onValueChanged.AddListener(value => instance.SetMasterVolume(value));
                    break;
                case "MusicSlider":
                    slider.value = PlayerPrefs.GetFloat("MusicVolume");
                    slider.onValueChanged.AddListener(value => instance.SetMusicVolume(value));
                    break;
                case "SFXSlider":
                    slider.value = PlayerPrefs.GetFloat("SFXVolume");
                    slider.onValueChanged.AddListener(value => instance.SetSfxVolume(value));
                    break;
            }
        }
    }

    private float ToDecibels(float volume) {
        float dB = (40f / 9f) * (1f - Mathf.Pow(10f, 1 - volume));
        dB = dB < -39f ? -80f : dB;
        return dB;
    }

    private void SetMenuButtons()
    {
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag("UIButton");
        foreach (var go in gameObjects)
        {
            go.GetComponent<Button>().onClick.AddListener(UIButtonSound);
        }
    }
}