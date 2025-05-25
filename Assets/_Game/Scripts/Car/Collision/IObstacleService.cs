public interface IObstacleService
{
    void RegisterObstacle(IObstacle obstacle);
    void UnregisterObstacle(IObstacle obstacle);
    ICollisionDetector GetCollisionDetector();
} 