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
    [SerializeField] private Transform steeringWheelPoint;
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private float maxSteeringWheelAngle = 90f;
    [SerializeField] private float steeringWheelSmoothSpeed = 5f;
    [SerializeField] private float steerPivotSmoothSpeed = 5f;
    [SerializeField] private float collisionRadius = 1f;
    [SerializeField] private Vector3 collisionSize = new Vector3(2f, 1f, 4f);
    [SerializeField] private Vector3 collisionCenterOffset = Vector3.zero;
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
    public Transform SteeringWheelPoint => steeringWheelPoint;
    public Transform SteeringWheel => steeringWheel != null ? steeringWheel : steeringWheelPoint;
    public float MaxSteeringWheelAngle => maxSteeringWheelAngle;
    public float SteeringWheelSmoothSpeed => steeringWheelSmoothSpeed;
    public float SteerPivotSmoothSpeed => steerPivotSmoothSpeed;
    public float CollisionRadius => collisionRadius;
    public Vector3 CollisionSize => collisionSize;
    public Vector3 CollisionCenterOffset => collisionCenterOffset;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(collisionCenterOffset, collisionSize);
        Gizmos.matrix = Matrix4x4.identity;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + collisionCenterOffset, collisionRadius);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawCube(collisionCenterOffset, collisionSize);
        Gizmos.matrix = Matrix4x4.identity;
        
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position + collisionCenterOffset, collisionRadius);
    }
} 