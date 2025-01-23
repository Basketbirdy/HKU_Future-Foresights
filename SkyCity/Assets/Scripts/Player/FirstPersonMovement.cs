using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState { WALKING, FLYING, MENU }
public class FirstPersonMovement : MonoBehaviour
{
    [SerializeField] private PlayerState currentState;

    [Header("Movement")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;  // yVelocity = mathf.sqrt(jumpHeight * -2f * gravity)
    [SerializeField] private float sprintMultiplier;
    private InputAction move;
    private InputAction sprint;

    [Header("Look")]
    [SerializeField] private Transform camPoint;
    [SerializeField] private float xSensitivity;
    [SerializeField] private float ySensitivity;
    [SerializeField] private Vector2 upDownRange;
    float lookXRotation;
    float lookYRotation;
    private InputAction look;

    [Header("Jump")]
    [SerializeField] private float checkOffset;
    [SerializeField] private Vector3 checkSize;
    [SerializeField] private LayerMask groundedLayers;
    private InputAction jump;
    private bool isGrounded;

    [Header("Fly")]
    [SerializeField] private float flyingSpeed;
    private InputAction fly;

    [Header("Physics")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Input")]
    [SerializeField] private PlayerInput input;

    [SerializeField] private bool moveOnStart;

    private Vector3 velocity;

    // current variables
    private bool movementEnabled;
    private bool affectedByGravity;
    private float currentSpeed;
    private float speedMultiplier;
    private bool isSprinting;

    private void OnEnable()
    {
        move.Enable();
        look.Enable();
        jump.Enable();
        fly.Enable();
        sprint.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        jump.Disable();
        fly.Disable();
        sprint.Disable();
    }

    private void Awake()
    {
        move = input.actions.FindAction("Move");
        look = input.actions.FindAction("Look");
        jump = input.actions.FindAction("Jump");
        fly = input.actions.FindAction("Interact");
        sprint = input.actions.FindAction("Sprint");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (moveOnStart) { movementEnabled = true; }
        else { movementEnabled = false; }

        affectedByGravity = true;

        ChangeState(PlayerState.WALKING);
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();

        OnUpdateState();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckBox(transform.position + transform.up * checkOffset, checkSize, Quaternion.identity, groundedLayers);
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (movementEnabled == false) { return; }

        Vector2 movementInput = move.ReadValue<Vector2>();
        Vector3 movement = camPoint.transform.right * movementInput.x + camPoint.transform.forward * movementInput.y;
        characterController.Move(movement * currentSpeed * Time.deltaTime);

        if (jump.triggered) { OnJump(); }

        if (affectedByGravity)
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void RotatePlayer()
    {
        Vector2 lookDelta = look.ReadValue<Vector2>();
        lookXRotation = lookDelta.x * xSensitivity * Time.deltaTime;
        transform.Rotate(0, lookXRotation, 0);

        lookYRotation -= lookDelta.y * ySensitivity * Time.deltaTime;
        lookYRotation = Mathf.Clamp(lookYRotation, upDownRange.x, upDownRange.y);
        camPoint.transform.localRotation = Quaternion.Euler(lookYRotation, 0, 0);
    }

    private void ChangeState(PlayerState _state) 
    { 
        OnExitState();

        currentState = _state;

        OnEnterState();
    }

    private void OnEnterState()
    {
        switch (currentState)
        {
            case PlayerState.WALKING:
                currentSpeed = speed;
                break;
            case PlayerState.FLYING:
                currentSpeed = flyingSpeed;
                affectedByGravity = false;
                break;
            case PlayerState.MENU:
                movementEnabled = false;
                break;
        }
    }

    private void OnUpdateState()
    {
        switch (currentState)
        {
            case PlayerState.WALKING:
                if (fly.triggered) { ChangeState(PlayerState.FLYING); }
                
                if (sprint.triggered) 
                {
                    if (!isSprinting) 
                    { 
                        isSprinting = true;
                        currentSpeed = speed * sprintMultiplier;
                    }
                    else
                    {
                        isSprinting = false;
                        currentSpeed = speed / sprintMultiplier;
                    }
                }

                break;
            case PlayerState.FLYING:
                if (fly.triggered) { ChangeState(PlayerState.WALKING); }

                if (sprint.triggered)
                {
                    if (!isSprinting)
                    {
                        isSprinting = true;
                        currentSpeed = flyingSpeed * sprintMultiplier;
                    }
                    else
                    {
                        isSprinting = false;
                        currentSpeed = flyingSpeed / sprintMultiplier;
                    }
                }

                break;
            case PlayerState.MENU:
                break;
        }
    }

    private void OnExitState()
    {
        switch (currentState)
        {
            case PlayerState.WALKING:
                break;
            case PlayerState.FLYING:
                affectedByGravity = true;
                break;
            case PlayerState.MENU:
                movementEnabled = true;
                break;
        }
    }

    private void OnJump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void OnDrawGizmos()
    {
        if (moveOnStart == false) { return; }
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position + transform.up * checkOffset, checkSize);
    }
}
