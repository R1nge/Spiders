using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpiderMove : MonoBehaviour
{
    private float _moveSpeed;
    private float _rotationSpeed;
    private Camera _camera;
    private Vector2 _targetDirection;
    private float _coolDown;
    private Transform _cachedTransform;

    private NativeArray<float> _coolDownResult;
    private NativeArray<Vector2> _targetDirectionResult;
    private NativeArray<Vector2> _positionResult;
    private NativeArray<Quaternion> _rotationResult;

    private JobHandle _jobHandle;

    private void Awake()
    {
        _cachedTransform = transform;
        _camera = Camera.main;
        _targetDirection = Vector2.up;
        _moveSpeed = Entrypoint.Instance.Random.NextFloat(2f, 6f);
        _rotationSpeed = Entrypoint.Instance.Random.NextFloat(90f, 180f);

        _coolDownResult = new NativeArray<float>(1, Allocator.Persistent);
        _targetDirectionResult = new NativeArray<Vector2>(1, Allocator.Persistent);
        _positionResult = new NativeArray<Vector2>(1, Allocator.Persistent);
        _rotationResult = new NativeArray<Quaternion>(1, Allocator.Persistent);
    }

    private void Update()
    {
        var screenPoint = _camera.WorldToScreenPoint(_cachedTransform.position);

        var job = new SpiderJob(
            _targetDirection,
            _coolDown,
            _moveSpeed,
            _rotationSpeed,
            Time.deltaTime,
            (uint)Entrypoint.Instance.RandomIndex,
            screenPoint,
            _camera.pixelWidth,
            _camera.pixelHeight,
            _cachedTransform.rotation,
            _cachedTransform.position,
            _coolDownResult,
            _targetDirectionResult,
            _positionResult,
            _rotationResult
        );

        _jobHandle = job.Schedule();
    }

    private void LateUpdate()
    {
        _jobHandle.Complete();

        _targetDirection = _targetDirectionResult[0];
        _coolDown = _coolDownResult[0];
        transform.position = _positionResult[0];
        transform.rotation = _rotationResult[0];
    }

    private void OnDestroy()
    {
        _coolDownResult.Dispose();
        _targetDirectionResult.Dispose();
        _positionResult.Dispose();
        _rotationResult.Dispose();
    }
}