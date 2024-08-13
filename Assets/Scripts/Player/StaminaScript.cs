using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StaminaScript : MonoBehaviour
{

    [Header("Stamina Main Param")] 
    public float playersStamina = 100.0f;
    [SerializeField] public float staminaMax = 100.0f;
    public bool hasRegenerated = true;
    public bool weRow = false;
    public float replenishBuoys = 10f;
    
    [Header("Stamina Regen Param")] 
    [Range(0, 50)] [SerializeField] private float staminaDrain = .5f;
    [Range(0, 50)] [SerializeField] private float staminaRegen = .5f;
    [SerializeField] private float timer;
    [Range(0, 10)][SerializeField] private float multiplyRegen = 2f;

    [Header("Stamina UI")] 
    private Image staminaBackground;
    [SerializeField] private Gradient _gradientColor;
    private Image staminaProgressBar;

    [Range(0, 10)] [SerializeField] private float blinkSpeed = 2f;
    [HideInInspector] public PlayerController _playerController;
    [SerializeField] private AudioSource PickUpSource;
    private PlayerAnimator playerAnimator;
    
    private Buoyancy _buoyancy;
    private Vector3 _streamDirection;

    [SerializeField] private GameObject boatCullMesh;
    
    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _buoyancy = GetComponent<Buoyancy>();
        playerAnimator = GetComponent<PlayerAnimator>();

    }
    
    void Update()
    {
        _streamDirection = _buoyancy.SampleStream(transform.position);
        StaminaRegenerated();
    }

    public Vector3 GetStreamDirection()
    {
        return _streamDirection;
    }
    
    public void StaminaRowing()
    {
        if (weRow && hasRegenerated)
        {
            playersStamina -= staminaDrain * Time.smoothDeltaTime;
            updateStamina();
            if (playersStamina <= 0)
            {
                playerAnimator.FallOff(true);
                playersStamina = 0;
                weRow = false;
                hasRegenerated = false;
                _playerController.enabled = false;
                boatCullMesh.SetActive(false);
            }
        }
    }

    public void StaminaRegenerated()
    {
        if (!hasRegenerated)
        {
            staminaBackground.color = Color.Lerp(Color.blue, Color.red, Mathf.PingPong(Time.time * blinkSpeed, 1));
        }
       
        if (!weRow && !hasRegenerated)
        {
            
            if (playersStamina <= staminaMax)
            {
                playersStamina += (staminaRegen * multiplyRegen)  * Time.smoothDeltaTime;
                updateStamina();
                if (playersStamina >= staminaMax)
                {
                    playersStamina = staminaMax;
                    staminaBackground.color = Color.white;
                    StartCoroutine(RegenWait());
                    
                }
            }
        }
        else if (!weRow)
        {
            if (playersStamina <= staminaMax)
            {
                playersStamina += staminaRegen  * Time.smoothDeltaTime;
                updateStamina();
                if (playersStamina >= staminaMax)
                {
                    playersStamina = staminaMax;
                    staminaBackground.color = Color.white;
                    StartCoroutine(RegenWait());
                    
                }
            }
        }
        {
            
        }
    }
    
    public void OnCollisionEnter(Collision other)
    {
     
        if (other.gameObject.CompareTag("Buoy"))
        {
            if (PickUpSource && !PickUpSource.isPlaying)
            {
                PickUpSource.Play();
            }
            //adds to the stamina bar in the scene
            if (playersStamina <= staminaMax)
            {
                playersStamina += replenishBuoys;
                updateStamina();
                Destroy(other.gameObject);
                PickUpSource.Play();
            }
            if(playersStamina >= staminaMax)
            {
                playersStamina = staminaMax;
                updateStamina();
                Destroy(other.gameObject);
                PickUpSource.Play();

            }
        }
    }
    
    void updateStamina()
    {
        staminaProgressBar.fillAmount = playersStamina / staminaMax;
        staminaProgressBar.color = _gradientColor.Evaluate(staminaProgressBar.fillAmount);
    }
    
    IEnumerator RegenWait()
    {
       
        yield return new WaitForSecondsRealtime(timer);
        playerAnimator.FallOff(false);
        _playerController.enabled = true;
        hasRegenerated = true;
        yield return new WaitForSecondsRealtime(1.75f);
        boatCullMesh.SetActive(true);
    }

    public void AssignStaminaUI(Image staminaProgressBar, Image staminaBackground) {
        this.staminaProgressBar = staminaProgressBar;
        this.staminaBackground = staminaBackground;
    }
    
}
