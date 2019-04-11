using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(CursorController))]
public class PlayerController : MonoBehaviour
{
    PlayerMovement playerMovement;
    CursorController cursorController;


    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        cursorController = GetComponent<CursorController>();
    }
    

    void Update()
    {
        playerMovement.HandleInput();
        cursorController.HandleInputs();
    }
}
