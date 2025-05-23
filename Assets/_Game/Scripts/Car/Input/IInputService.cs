using UnityEngine;

public interface IInputService
{
    Vector2 Steering { get; }
    bool Gas { get; }
    bool Brake { get; }
} 