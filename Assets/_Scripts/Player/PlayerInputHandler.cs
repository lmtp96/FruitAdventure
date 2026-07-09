using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputSet inputActions;
    private PlayerMovement playerMovement;

    public Vector2 moveInput { get; private set; }

    // Lay Player va tao bo input rieng cho object nay
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        inputActions = new PlayerInputSet();
    }

    // Bat input va noi cac action voi ham xu ly
    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Movement.performed += OnMovementPerformed;
        inputActions.Player.Movement.canceled += OnMovementCanceled;
        inputActions.Player.Jump.performed += OnJumpPerformed;

        inputActions.UI.Pause.performed += OnPausePerformed;

    }

    // Ngung nhan input khi Player khong con hoat dong
    private void OnDisable()
    {
        inputActions.Player.Movement.performed -= OnMovementPerformed;
        inputActions.Player.Movement.canceled -= OnMovementCanceled;
        inputActions.Player.Jump.performed -= OnJumpPerformed;

        inputActions.UI.Pause.performed -= OnPausePerformed;

        moveInput = Vector2.zero;
        inputActions.Disable();
    }


    // Luu input di chuyen cho Player.cs su dung
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }


    /// Reset input khi tha phim de Player dung lai
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    // Goi Player.cs xu ly jump
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        playerMovement.HandleJumpInput();
    }

    // Goi UI_InGame xu ly pause menu
    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        UI_InGame.instance.PauseButton();
    }

    // Bat lai input gameplay cua Player, dung khi thoat pause
    public void EnablePlayerInput()
    {
        inputActions.Player.Enable();
    }

    // Tat input gameplay cua Player, dung khi dang pause
    public void DisablePlayerInput()
    {
        moveInput = Vector2.zero;
        inputActions.Player.Disable();
    }

}
