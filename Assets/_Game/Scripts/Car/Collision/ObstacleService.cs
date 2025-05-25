public class ObstacleService : IObstacleService
{
    private readonly CollisionDetector _collisionDetector;

    public ObstacleService()
    {
        _collisionDetector = new CollisionDetector();
    }

    public void RegisterObstacle(IObstacle obstacle)
    {
        _collisionDetector.AddObstacle(obstacle);
    }

    public void UnregisterObstacle(IObstacle obstacle)
    {
        _collisionDetector.RemoveObstacle(obstacle);
    }

    public ICollisionDetector GetCollisionDetector()
    {
        return _collisionDetector;
    }
} 