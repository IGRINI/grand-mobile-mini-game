public class CarModel : ICarModel
{
    public float Acceleration { get; }
    public float BrakeForce { get; }
    public float MaxSpeed { get; }
    public float MaxReverseSpeed { get; }
    public float TurnSpeed { get; }
    public float CurrentSpeed { get; set; }

    public CarModel(float acceleration, float brakeForce, float maxSpeed, float maxReverseSpeed, float turnSpeed)
    {
        Acceleration = acceleration;
        BrakeForce = brakeForce;
        MaxSpeed = maxSpeed;
        MaxReverseSpeed = maxReverseSpeed;
        TurnSpeed = turnSpeed;
        CurrentSpeed = 0;
    }
} 