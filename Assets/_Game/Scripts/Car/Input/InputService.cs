using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputService : IInputService, IInitializable, IDisposable
{
    private readonly InputActionAsset _asset;
    private InputAction _steeringAction;
    private InputAction _gasAction;
    private InputAction _brakeAction;
    private InputAction _interactAction;
    private InputAction _crouchAction;

    public InputService(InputActionAsset asset)
    {
        _asset = asset;
    }

    public Vector2 MoveDirection => _steeringAction.ReadValue<Vector2>();
    public bool Gas => _gasAction.ReadValue<float>() > 0;
    public bool Brake => _brakeAction.ReadValue<float>() > 0;
    public bool Interact => _interactAction.ReadValue<float>() > 0;
    public bool Crouch => _crouchAction.ReadValue<float>() > 0;

    public void Initialize()
    {
        _steeringAction = _asset.FindAction("Move");
        _gasAction = _asset.FindAction("Click");
        _brakeAction = _asset.FindAction("RightClick");
        _interactAction = _asset.FindAction("Interact");
        _crouchAction = _asset.FindAction("Crouch");
        _steeringAction.Enable();
        _gasAction.Enable();
        _brakeAction.Enable();
        _interactAction.Enable();
        _crouchAction.Enable();
    }

    public void Dispose()
    {
        _steeringAction.Disable();
        _gasAction.Disable();
        _brakeAction.Disable();
        _interactAction.Disable();
        _crouchAction.Disable();
    }
} 