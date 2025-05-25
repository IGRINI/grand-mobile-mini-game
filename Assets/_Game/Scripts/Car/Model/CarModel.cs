public class CarModel : ICarModel
{
    public float Acceleration { get; }
    public float BrakeForce { get; }
    public float MaxSpeed { get; }
    public float MaxReverseSpeed { get; }
    public float TurnSpeed { get; }
    public float MinSpeedToTurn { get; }
    public float CurrentSpeed { get; set; }
    public float MaxTurnRate { get; }
    public float TurnRateSpeedFactor { get; }
    public float BaseCollisionDamage { get; }
    public float SelfDamageMultiplier { get; }
    public float SpeedReductionPerEnemy { get; }

    public CarModel(float acceleration, float brakeForce, float maxSpeed, float maxReverseSpeed, float turnSpeed, float minSpeedToTurn, float maxTurnRate, float turnRateSpeedFactor, float baseCollisionDamage, float selfDamageMultiplier, float speedReductionPerEnemy)
    {
        Acceleration = acceleration;
        BrakeForce = brakeForce;
        MaxSpeed = maxSpeed;
        MaxReverseSpeed = maxReverseSpeed;
        TurnSpeed = turnSpeed;
        MinSpeedToTurn = minSpeedToTurn;
        CurrentSpeed = 0;
        MaxTurnRate = maxTurnRate;
        TurnRateSpeedFactor = turnRateSpeedFactor;
        BaseCollisionDamage = baseCollisionDamage;
        SelfDamageMultiplier = selfDamageMultiplier;
        SpeedReductionPerEnemy = speedReductionPerEnemy;
    }
} 