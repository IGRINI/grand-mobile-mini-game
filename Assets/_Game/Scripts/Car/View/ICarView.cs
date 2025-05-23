using UnityEngine;
using System.Collections.Generic;

public interface ICarView
{
    Transform Transform { get; }
    IReadOnlyList<Transform> Wheels { get; }
    IReadOnlyList<Transform> SteerPivots { get; }
    Transform RearAxisCenter { get; }
    float MaxSteerAngle { get; }
    Vector3 WheelSpinAxis { get; }
    IReadOnlyList<Transform> CharacterPivots { get; }
} 