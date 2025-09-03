using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public struct SpiderJob : IJob
{
    private Vector2 _targetDirection;
    private float _changeDirectionCooldown;
    private readonly float _moveSpeed;
    private readonly float _rotationSpeed;
    private readonly float _deltaTime;
    private readonly Vector2 _screenPoint;
    private readonly float _screenHeight;
    private readonly float _screenWidth;
    private Quaternion _rotation;
    private readonly Vector2 _position;
    private Unity.Mathematics.Random _random;
    [WriteOnly] private NativeArray<float> _coolDownResult;
    [WriteOnly] private NativeArray<Vector2> _targetDirectionResult;
    [WriteOnly] private NativeArray<Vector2> _positionResult;
    [WriteOnly] private NativeArray<Quaternion> _rotationResult;

    public SpiderJob(
        Vector2 targetDirection,
        float changeDirectionCooldown,
        float moveSpeed,
        float rotationSpeed,
        float deltaTime,
        uint seed,
        Vector2 screenPoint,
        int screenWidth,
        int screenHeight,
        Quaternion rotation,
        Vector2 position,
        NativeArray<float> coolDownResult,
        NativeArray<Vector2> targetDirectionResult,
        NativeArray<Vector2> positionResult,
        NativeArray<Quaternion> rotationResult)
    {
        _targetDirection = targetDirection;
        _changeDirectionCooldown = changeDirectionCooldown;
        _moveSpeed = moveSpeed;
        _rotationSpeed = rotationSpeed;
        _deltaTime = deltaTime;
        _screenPoint = screenPoint;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _rotation = rotation;
        _position = position;
        _coolDownResult = coolDownResult;
        _targetDirectionResult = targetDirectionResult;
        _positionResult = positionResult;
        _rotationResult = rotationResult;
        _random = new Unity.Mathematics.Random(seed);
    }


    public void Execute()
    {
        TickTimer();
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetPosition();
    }

    private void UpdateTargetDirection()
    {
        ChangeDirection();
        HandleOffScreen();

        _targetDirectionResult[0] = _targetDirection;
    }

    private void TickTimer()
    {
        _changeDirectionCooldown -= _deltaTime;
    }

    private void ChangeDirection()
    {
        if (_changeDirectionCooldown <= 0)
        {
            float newAngle = _random.NextFloat(-90f, 90f);
            Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
            _targetDirection = rotation * _targetDirection;

            _changeDirectionCooldown = _random.NextFloat(1f, 5f);
        }

        _coolDownResult[0] = _changeDirectionCooldown;
    }

    private void HandleOffScreen()
    {
        if (_screenPoint.x < 0 && _targetDirection.x < 0 ||
            (_screenPoint.x > _screenWidth && _targetDirection.x > 0))
        {
            _targetDirection = new Vector2(-_targetDirection.x, _targetDirection.y);
        }

        if (_screenPoint.y < 0 && _targetDirection.y < 0 ||
            (_screenPoint.y > _screenHeight && _targetDirection.y > 0))
        {
            _targetDirection = new Vector2(_targetDirection.x, -_targetDirection.y);
        }
    }

    private void RotateTowardsTarget()
    {
        var targetRotation = Quaternion.LookRotation(Vector3.forward, _targetDirection);
        _rotation = Quaternion.RotateTowards(_rotation, targetRotation, _rotationSpeed * _deltaTime);
        _rotationResult[0] = _rotation;
    }

    private void SetPosition()
    {
        Vector2 positionChange = _rotation * Vector2.up * _moveSpeed * _deltaTime;
        _positionResult[0] = _position + positionChange;
    }
}