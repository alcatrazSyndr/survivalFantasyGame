using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ClientInputController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private float _cameraInputSensitivity = 1f;

    [Header("Input Actions")]
    [SerializeField] private InputAction _movementInputAction = new InputAction();
    [SerializeField] private InputAction _cameraInputAction = new InputAction();

    [Header("Events")]
    public UnityEvent<Vector2> OnMovementInputChanged = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnCameraInputChanged = new UnityEvent<Vector2>();

    #region Movement Input

    public void ToggleMovementInput(bool toggle)
    {
        if (toggle)
        {
            _movementInputAction.Enable();

            _movementInputAction.performed += MovementInputChanged;
            _movementInputAction.canceled += MovementInputChanged;
        }
        else
        {
            _movementInputAction.performed -= MovementInputChanged;
            _movementInputAction.canceled -= MovementInputChanged;

            _movementInputAction.Disable();
        }
    }

    private void MovementInputChanged(InputAction.CallbackContext movementInput)
    {
        var input = movementInput.ReadValue<Vector2>();

        OnMovementInputChanged?.Invoke(input);
    }

    #endregion

    #region Camera Input

    public void ToggleCameraInput(bool toggle)
    {
        if (toggle)
        {
            _cameraInputAction.Enable();

            _cameraInputAction.performed += CameraInputChanged;
            _cameraInputAction.canceled += CameraInputChanged;
        }
        else
        {
            _cameraInputAction.performed += CameraInputChanged;
            _cameraInputAction.canceled += CameraInputChanged;

            _cameraInputAction.Disable();
        }
    }

    private void CameraInputChanged(InputAction.CallbackContext cameraInput)
    {
        var input = cameraInput.ReadValue<Vector2>();

        var output = input * _cameraInputSensitivity;

        OnCameraInputChanged?.Invoke(input);
    }

    #endregion
}
