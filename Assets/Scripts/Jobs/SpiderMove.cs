using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SpiderMove : MonoBehaviour
{
    private float _moveSpeed;
    private float _rotationSpeed;
    private Camera _camera;
    private Vector2 _targetDirection;
    private float _coolDown;

    private NativeArray<float> _coolDownResult;
    private NativeArray<Vector2> _targetDirectionResult;
    private NativeArray<Vector2> _positionResult;
    private NativeArray<Quaternion> _rotationResult;

    private JobHandle _jobHandle;

    private void Awake()
    {
        _camera = Camera.main;
        _targetDirection = Vector2.up;
        _moveSpeed = Random.Range(2f, 6f);
        _rotationSpeed = Random.Range(90f, 180f);

        _coolDownResult = new NativeArray<float>(1, Allocator.Persistent);
        _targetDirectionResult = new NativeArray<Vector2>(1, Allocator.Persistent);
        _positionResult = new NativeArray<Vector2>(1, Allocator.Persistent);
        _rotationResult = new NativeArray<Quaternion>(1, Allocator.Persistent);
    }

    private void Update()
    {
        var chachedTranform = transform;
        var screenPoint = _camera.WorldToScreenPoint(chachedTranform.position);

        var job = new SpiderJob(
            _targetDirection,
            _coolDown,
            _moveSpeed,
            _rotationSpeed,
            Time.deltaTime,
            (uint)Random.Range(1, 1000),
            screenPoint,
            _camera.pixelWidth,
            _camera.pixelHeight,
            chachedTranform.rotation,
            chachedTranform.position,
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

        if (_jobHandle.IsCompleted)
        {
            _targetDirection = _targetDirectionResult[0];
            _coolDown = _coolDownResult[0];
            transform.position = _positionResult[0];
            transform.rotation = _rotationResult[0];
        }
    }

    private void OnDestroy()
    {
        _coolDownResult.Dispose();
        _targetDirectionResult.Dispose();
        _positionResult.Dispose();
        _rotationResult.Dispose();
    }
}