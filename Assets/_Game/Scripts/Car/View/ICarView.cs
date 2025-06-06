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
    Transform SteeringWheelPoint { get; }
    Transform SteeringWheel { get; }
    float MaxSteeringWheelAngle { get; }
    float SteeringWheelSmoothSpeed { get; }
    float SteerPivotSmoothSpeed { get; }
    Transform CarBody { get; }
    float MaxLeanAngle { get; }
    float LeanSpring { get; }
    float LeanDamping { get; }
    float PitchSpring { get; }
    float PitchDamping { get; }
    float BasePitchAngle { get; }
    float AccelerationPitchFactor { get; }
    float MaxPitchAngle { get; }
    Transform HitTarget { get; }
    float CollisionRadius { get; }
    Vector3 CollisionSize { get; }
    Vector3 CollisionCenterOffset { get; }
} 