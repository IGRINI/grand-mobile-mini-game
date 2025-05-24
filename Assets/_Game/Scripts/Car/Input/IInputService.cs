using UnityEngine;

public interface IInputService
{
    Vector2 MoveDirection { get; }
    bool Gas { get; }
    bool Brake { get; }
    bool Interact { get; }
    bool Crouch { get; }
} 