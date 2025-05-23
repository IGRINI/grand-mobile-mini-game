using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CarController : IInitializable, IDisposable
{
    private readonly ICarModel _model;
    private readonly ICarView _view;
    private readonly IInputService _input;
    private CancellationTokenSource _cts;

    public CarController(ICarModel model, ICarView view, IInputService input)
    {
        _model = model;
        _view = view;
        _input = input;
    }

    public void Initialize()
    {
        _cts = new CancellationTokenSource();
        RunAsync(_cts.Token).Forget();
    }

    private async UniTask RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            UpdateCar(Time.deltaTime);
            await UniTask.Yield();
        }
    }

    private void UpdateCar(float deltaTime)
    {
        var steering = _input.Steering;
        if (steering.sqrMagnitude > 0)
        {
            var targetDir = new Vector3(steering.x, 0, steering.y);
            var targetRot = Quaternion.LookRotation(targetDir);
            _view.Transform.rotation = Quaternion.RotateTowards(_view.Transform.rotation, targetRot, _model.TurnSpeed * deltaTime);
        }
        float acceleration = 0;
        if (_input.Gas) acceleration = _model.Acceleration;
        else if (_input.Brake) acceleration = -_model.BrakeForce;
        _model.CurrentSpeed = Mathf.Clamp(_model.CurrentSpeed + acceleration * deltaTime, -_model.MaxReverseSpeed, _model.MaxSpeed);
        _view.Transform.position += _view.Transform.forward * _model.CurrentSpeed * deltaTime;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
} 