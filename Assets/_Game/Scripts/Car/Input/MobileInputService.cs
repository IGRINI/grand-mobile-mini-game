using UnityEngine;

public class MobileInputService : MonoBehaviour, IInputService
{
    [SerializeField] private TouchJoystick joystick;
    [SerializeField] private MobileButton gasButton;
    [SerializeField] private MobileButton brakeButton;

    public Vector2 MoveDirection => joystick != null ? joystick.Direction : Vector2.zero;
    public bool Gas => gasButton != null && gasButton.IsPressed;
    public bool Brake => brakeButton != null && brakeButton.IsPressed;
    public bool Interact => false;
    public bool Crouch => false;
} 