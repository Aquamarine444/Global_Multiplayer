using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
public class Example : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private float moveDirection = 0f;
    public Transform playerCamera;
    public UIManager uiManager;

    [ClientCallback]
    void Start()
    {
        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    [ClientCallback]  //only run on client
    private void Update()
    {
        if (!isLocalPlayer) return;
        transform.Translate(Vector3.right * moveDirection * moveSpeed *
        Time.deltaTime);
    }

    [TargetRpc]
    public void TargetShowWelcomeMessage(NetworkConnection target, string message)
    {
        if (uiManager != null)
        {
            uiManager.ShowMessage(message);
        }
    }
    [Command]
    void CmdChangeMyColour() //Command above it thus naming convention it starts with Cmd
    {
        Color newColor = Random.ColorHSV();
        RpcChangeColor(newColor);
    }

    [ClientRpc] //called by the server - perform the command asked and sync it across all clients (clinets can now see the change)
    void RpcChangeColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    public void OnColorChange(InputValue value) //new input system = On....
    {
        if (value.isPressed)
        {
            CmdChangeMyColour();
        }
    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>().x;
    }
}
