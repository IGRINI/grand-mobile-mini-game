public interface ICarModel
{
    float Acceleration { get; }
    float BrakeForce { get; }
    float MaxSpeed { get; }
    float MaxReverseSpeed { get; }
    float TurnSpeed { get; }
    float MinSpeedToTurn { get; }
    float CurrentSpeed { get; set; }
    float MaxTurnRate { get; }
    float TurnRateSpeedFactor { get; }
} 