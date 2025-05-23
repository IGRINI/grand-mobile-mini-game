using UnityEngine;
using System.Collections.Generic;

public class CarView : MonoBehaviour, ICarView
{
    public Transform Transform => transform;
    [SerializeField] private Transform[] wheels;
    [SerializeField] private Transform[] steerPivots;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private Vector3 wheelSpinAxis = Vector3.right;
    public IReadOnlyList<Transform> Wheels => wheels;
    public IReadOnlyList<Transform> SteerPivots => steerPivots;
    public float MaxSteerAngle => maxSteerAngle;
    public Vector3 WheelSpinAxis => wheelSpinAxis;
} 