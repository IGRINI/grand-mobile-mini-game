using UnityEngine;
using System.Collections.Generic;

public interface ICarView
{
    Transform Transform { get; }
    IReadOnlyList<Transform> Wheels { get; }
    IReadOnlyList<Transform> SteerPivots { get; }
    float MaxSteerAngle { get; }
    Vector3 WheelSpinAxis { get; }
} 