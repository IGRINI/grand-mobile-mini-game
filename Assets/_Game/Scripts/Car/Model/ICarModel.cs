public interface ICarModel
{
    float Acceleration { get; }
    float BrakeForce { get; }
    float MaxSpeed { get; }
    float MaxReverseSpeed { get; }
    float TurnSpeed { get; }
    float CurrentSpeed { get; set; }
} 