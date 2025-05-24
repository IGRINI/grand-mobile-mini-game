using UnityEngine;
using System.Collections.Generic;

public class CarView : MonoBehaviour, ICarView
{
    public Transform Transform => transform;
    [SerializeField] private Transform[] wheels;
    [SerializeField] private Transform[] steerPivots;
    [SerializeField] private Transform rearAxisCenter;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private Vector3 wheelSpinAxis = Vector3.right;
    [SerializeField] private Transform[] characterPivots;
    [SerializeField] private Transform carBody;
    [SerializeField] private float maxLeanAngle = 10f;
    [SerializeField] private float leanSpring = 6f;
    [SerializeField] private float leanDamping = 4f;
    [SerializeField] private float pitchSpring = 3f;
    [SerializeField] private float pitchDamping = 2f;
    [SerializeField] private float basePitchAngle = 2f;
    [SerializeField] private float accelerationPitchFactor = 8f;
    [SerializeField] private float maxPitchAngle = 10f;
    [SerializeField] private Transform hitTarget;
    public IReadOnlyList<Transform> Wheels => wheels;
    public IReadOnlyList<Transform> SteerPivots => steerPivots;
    public Transform RearAxisCenter => rearAxisCenter;
    public float MaxSteerAngle => maxSteerAngle;
    public Vector3 WheelSpinAxis => wheelSpinAxis;
    public IReadOnlyList<Transform> CharacterPivots => characterPivots;
    public Transform CarBody => carBody;
    public float MaxLeanAngle => maxLeanAngle;
    public float LeanSpring => leanSpring;
    public float LeanDamping => leanDamping;
    public float PitchSpring => pitchSpring;
    public float PitchDamping => pitchDamping;
    public float BasePitchAngle => basePitchAngle;
    public float AccelerationPitchFactor => accelerationPitchFactor;
    public float MaxPitchAngle => maxPitchAngle;
    public Transform HitTarget => hitTarget;
} 