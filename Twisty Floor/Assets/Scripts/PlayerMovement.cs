using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    Vector2 lookSensitivity = Vector2.one;
    [SerializeField]
    float maxLookPitch = 80.0f;
    [SerializeField]
    float walkSpeed = 5.0f;

    [SerializeField]
    Transform player;
    [SerializeField]
    Transform playerHead;

    [SerializeField]
    string lookXAxisName;
    [SerializeField]
    string lookYAxisName;
    [SerializeField]
    string moveXAxisName;
    [SerializeField]
    string moveYAxisName;

    float yaw;
    float pitch;
    

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    public void HandleInput()
    {
        HandleLookInput();
        HandleMoveInput();
    }


    private void HandleLookInput()
    {
        yaw += Input.GetAxisRaw(lookXAxisName) * lookSensitivity.x;
        pitch -= Input.GetAxisRaw(lookYAxisName) * lookSensitivity.y;

        pitch = Mathf.Clamp(pitch, -maxLookPitch, maxLookPitch);

        player.localRotation = Quaternion.Euler(0, yaw, 0);
        playerHead.localRotation = Quaternion.Euler(pitch, 0, 0);
    }


    private void HandleMoveInput()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw(moveXAxisName), 0, Input.GetAxisRaw(moveYAxisName));
        Vector3 moveVector = moveInput.normalized * walkSpeed * Time.deltaTime;
        player.localPosition += player.localRotation * moveVector;
    }
}
