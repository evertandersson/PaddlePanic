using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float rowForce = 15f;
    [SerializeField, Range(0f, 100f)] private float breakForce = 15f;

    private Rigidbody rb;

    [Header("Offsets")]
    [SerializeField] private Vector3 rowOffset = new Vector3(1f, 0f, 0f);
    [SerializeField] private Vector3 brakeOffset = new Vector3(1.5f, 0f, 0f);

    private Vector3 _streamDirection;

    private PlayerInput input;
    public float rightButton = 0;
    public float leftButton = 0;

    private PlayerAnimator playerAnimator;
    private StaminaScript _staminaScript;
    private Buoyancy _buoyancy = null;

    private bool invertControls = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new PlayerInput();
        playerAnimator = GetComponent<PlayerAnimator>();
        _staminaScript = GetComponent<StaminaScript>();
        _buoyancy = GetComponent<Buoyancy>();
        invertControls = PlayerPrefs.GetInt("InvertControls") == 1;
    }

    #region Inputs

    private void OnEnable()
    {
        input.Enable();
        //Boosts
        if (!invertControls) {
            input.Player.Right.performed += OnRightPerformed;
            input.Player.Right.canceled += OnRightCancelled;
            input.Player.Left.performed += OnLeftPerformed;
            input.Player.Left.canceled += OnLeftCancelled;
        } else {
            input.Player.Left.performed += OnRightPerformed;
            input.Player.Left.canceled += OnRightCancelled;
            input.Player.Right.performed += OnLeftPerformed;
            input.Player.Right.canceled += OnLeftCancelled;
        }

    }
    private void OnDisable()
    {
        input.Disable();
        //Boosts
        input.Player.Right.performed -= OnRightPerformed;
        input.Player.Right.canceled -= OnRightCancelled;
        input.Player.Left.performed -= OnLeftPerformed;
        input.Player.Left.canceled -= OnLeftCancelled;
    }

    //Boosts
    private void OnRightPerformed(InputAction.CallbackContext value)
    {
        rightButton = value.ReadValue<float>();
    }
    private void OnRightCancelled(InputAction.CallbackContext value)
    {
        rightButton = 0;
    }
    private void OnLeftPerformed(InputAction.CallbackContext value)
    {
        leftButton = value.ReadValue<float>();
    }
    private void OnLeftCancelled(InputAction.CallbackContext value)
    {
        leftButton = 0;
    }
    #endregion

    public Vector3 GetStreamDirection()
    {
        return _streamDirection;
    }

    private void FixedUpdate()
    {
        _streamDirection = _buoyancy.SampleStream(transform.position);

        //transform.forward = Vector3.Lerp(transform.forward, Vector3.forward, 0.1f).normalized;
        
        CheckForRow();
        CheckForBrake();
    }

    void CheckForRow()
    {
        if (playerAnimator.paddle1playing && playerAnimator.paddle2playing && _buoyancy.GetIsSubmerged())
        {
            RowForce(0);
        }
        if (playerAnimator.paddle1playing && _buoyancy.GetIsSubmerged())
        {
            RowForce(-1);
        }
        if (playerAnimator.paddle2playing && _buoyancy.GetIsSubmerged())
        {
            RowForce(1);
        }
        if (leftButton < 1 && rightButton < 1)
        {
            _staminaScript.weRow = false;
        }
    }

    void CheckForBrake() 
    {
        float speed = rb.velocity.magnitude;
        Vector3 force = -((_streamDirection + transform.forward) / 2 * breakForce) / speed;

        if (leftButton < 0 && _buoyancy.GetIsSubmerged())
        {
            //rb.AddForceAtPosition(-_streamDirection * breakForce * speed, transform.position + transform.rotation * new Vector3(-offset.x, offset.y, offset.z)); //Adds a negative force at left paddle,
            rb.AddForceAtPosition(force, transform.position + transform.rotation * new Vector3(-brakeOffset.x, brakeOffset.y, brakeOffset.z));                                                                                                                                       //unsure if this is best way to do it so feel free to change if there's better ways - Linus
        }
        if (rightButton < 0 && _buoyancy.GetIsSubmerged())
        {
            //rb.AddForceAtPosition(-_streamDirection * breakForce * speed, transform.position + transform.rotation * new Vector3(offset.x, offset.y, offset.z)); //Adds a negative force at right paddle,
            rb.AddForceAtPosition(force, transform.position + transform.rotation * new Vector3(brakeOffset.x, brakeOffset.y, brakeOffset.z));      //unsure if this is best way to do it so feel free to change if there's better ways - Linus
        }
    }

    void RowForce(int direction)
    {
        _staminaScript.weRow = true;
        rb.AddForceAtPosition(transform.forward * rowForce, transform.position + transform.rotation * new Vector3(rowOffset.x * direction, rowOffset.y, rowOffset.z));
        //_staminaScript.StaminaRowing(boost);
        if (_staminaScript)
            _staminaScript.StaminaRowing();
    }
}