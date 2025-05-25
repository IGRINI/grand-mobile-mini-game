using UnityEngine;

public class CarCollisionEffect : ICollisionEffect
{
    private readonly ICarView _carView;
    private readonly float _collisionPitchIntensity;
    private readonly float _collisionDuration;
    
    private float _collisionTimer;
    private float _targetCollisionPitch;
    private float _currentCollisionPitch;
    private float _collisionPitchVelocity;

    public CarCollisionEffect(ICarView carView, float collisionPitchIntensity = 15f, float collisionDuration = 0.5f)
    {
        _carView = carView;
        _collisionPitchIntensity = collisionPitchIntensity;
        _collisionDuration = collisionDuration;
    }

    public void ApplyCollisionEffect(Vector3 collisionNormal, float speed, float maxSpeed)
    {
        Vector3 carForward = _carView.Transform.forward;
        Vector3 carRight = _carView.Transform.right;
        
        float forwardDot = Vector3.Dot(carForward, collisionNormal);
        float rightDot = Vector3.Dot(carRight, collisionNormal);
        
        float speedFactor = Mathf.Clamp01(Mathf.Abs(speed) / maxSpeed);
        float impactIntensity = _collisionPitchIntensity * speedFactor;
        
        if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
        {
            if (forwardDot > 0.3f)
            {
                _targetCollisionPitch = -impactIntensity;
            }
            else if (forwardDot < -0.3f)
            {
                _targetCollisionPitch = impactIntensity;
            }
        }
        else
        {
            _targetCollisionPitch = 0f;
        }
        
        _collisionTimer = _collisionDuration * (0.5f + speedFactor * 0.5f);
    }

    public void UpdateCollisionEffect(float deltaTime)
    {
        if (_collisionTimer > 0f)
        {
            _collisionTimer -= deltaTime;
            
            float normalizedTime = 1f - (_collisionTimer / _collisionDuration);
            float dampingFactor = Mathf.Exp(-normalizedTime * 5f);
            
            _currentCollisionPitch = Mathf.SmoothDamp(
                _currentCollisionPitch, 
                _targetCollisionPitch * dampingFactor, 
                ref _collisionPitchVelocity, 
                0.1f, 
                Mathf.Infinity, 
                deltaTime
            );
        }
        else
        {
            _currentCollisionPitch = Mathf.SmoothDamp(
                _currentCollisionPitch, 
                0f, 
                ref _collisionPitchVelocity, 
                0.2f, 
                Mathf.Infinity, 
                deltaTime
            );
        }
    }

    public float GetCollisionPitch()
    {
        return _currentCollisionPitch;
    }
} 