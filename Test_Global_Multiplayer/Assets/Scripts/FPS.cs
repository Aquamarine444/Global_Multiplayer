using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))] //automatically adds CharacterController to object that has this script
public class FPSPlayer : NetworkBehaviour //NetworkBehaviour - class that comes/inherited from Mirror
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform playerCamera;

    [Header("Private Stuff")]
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float cameraPitch = 0f;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!isLocalPlayer) //if not this computer's player, disable camera
        {
            playerCamera.gameObject.SetActive(false);
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!isLocalPlayer) return; // return stops the code

        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        if (!isLocalPlayer) return;

        // Translate move input to world space
        Vector3 move = transform.right * moveInput.x + transform.forward *
        moveInput.y;
        move *= moveSpeed;

        // Apply gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (!isLocalPlayer) return;

        float mouseX = lookInput.x * lookSpeed * Time.deltaTime;
        float mouseY = lookInput.y * lookSpeed * Time.deltaTime;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void OnMove(InputValue value) //connected to InputSystem ActionMap - get inputvalue to be able to use it in code(case sensitive)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value) //connected to InputSystem ActionMap
    {
        lookInput = value.Get<Vector2>();
    }
}
