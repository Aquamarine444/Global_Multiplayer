using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))] //automatically adds CharacterController to object that has this script
public class FPSPlayer : NetworkBehaviour //NetworkBehaviour - class that comes/inherited from Mirror
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float lookSpeed = 5f;
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

    [Header("Shooting Stuff")]
    public Transform lazerTransform;
    public TrailRenderer lazerBeam;
    private bool isShooting;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!isLocalPlayer) //if not this computer's player, disable camera
        {
            playerCamera.gameObject.SetActive(false);
            gameObject.GetComponent<PlayerInput>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<PlayerInput>().enabled = true;
        }

        // Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
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

    public void HandleShoot()
    {
        if (!isLocalPlayer) return;
        Ray ray = new Ray(lazerTransform.position, lazerTransform.forward);
        // Instantiate the visual beam
        TrailRenderer beam = Instantiate(lazerBeam, lazerTransform.position, Quaternion.identity);
        beam.AddPosition(lazerTransform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
        {
            beam.transform.position = hit.point;
            // Try to damage a player if hit
            var playerStats = hit.collider.gameObject.GetComponent<PlayerStats>();
            if (playerStats)
            {
                playerStats.Damage(20); // Example damage amount
            }
        }
        else
        {
            beam.transform.position = lazerTransform.position + lazerTransform.forward * 50f;
        }
    }

    public void OnMove(InputValue value) //connected to InputSystem ActionMap - get inputvalue to be able to use it in code(case sensitive)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value) //connected to InputSystem ActionMap
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            HandleShoot();
        }
    }
}
