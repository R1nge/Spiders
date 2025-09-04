using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace MorpehECS
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(SpiderMoveSystem))]
    public class SpiderMoveSystem : UpdateSystem
    {
        private Filter _moveFilter;
        private Stash<SpiderMoveComponent> _spiderMoveStash;

        private Camera _camera;
        private float _screenWidth, _screenHeight;

        public override void OnAwake()
        {
            ComponentId<SpiderMoveComponent>.StashSize = Entrypoint.Instance.SpidersCount;

            _moveFilter = World.Filter.With<SpiderMoveComponent>().Build();
            _spiderMoveStash = World.GetStash<SpiderMoveComponent>();
            _camera = Camera.main;
            _screenWidth = _camera.pixelWidth;
            _screenHeight = _camera.pixelHeight;

            foreach (var entity in _moveFilter)
            {
                ref var spiderMoveComponent = ref _spiderMoveStash.Get(entity);
                spiderMoveComponent.moveSpeed = Entrypoint.Instance.Random.NextFloat(2f, 6f);
                spiderMoveComponent.rotateSpeed = Entrypoint.Instance.Random.NextFloat(90f, 180f);
                spiderMoveComponent.direction = Vector2.up;
                Entrypoint.Instance.PickNewRandom();
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _moveFilter)
            {
                ref var move = ref _spiderMoveStash.Get(entity);
                var screenPoint = _camera.WorldToScreenPoint(move.transform.position);
                var speed = move.moveSpeed;
                TickTimer(ref move, deltaTime);
                ChangeDirection(ref move);
                HandleOffScreen(ref move, screenPoint);
                RotateTowardsTarget(ref move, deltaTime);
                move.transform.Translate(move.transform.up * speed * deltaTime, Space.World);
            }
        }

        private void TickTimer(ref SpiderMoveComponent moveComponent, float delta)
        {
            moveComponent.changeDirectionCooldown -= delta;
        }

        private void ChangeDirection(ref SpiderMoveComponent moveComponent)
        {
            if (moveComponent.changeDirectionCooldown <= 0)
            {
                var newAngle = Entrypoint.Instance.Random.NextFloat(-90f, 90f);
                Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
                moveComponent.direction = rotation * moveComponent.direction;

                moveComponent.changeDirectionCooldown = Entrypoint.Instance.Random.NextFloat(1f, 5f);
                Entrypoint.Instance.PickNewRandom();
            }
        }

        private void HandleOffScreen(ref SpiderMoveComponent moveComponent, Vector3 screenPoint)
        {
            if (screenPoint.x < 0 && moveComponent.direction.x < 0 ||
                (screenPoint.x > _screenWidth && moveComponent.direction.x > 0))
            {
                moveComponent.direction = new Vector2(-moveComponent.direction.x, moveComponent.direction.y);
            }

            if (screenPoint.y < 0 && moveComponent.direction.y < 0 ||
                (screenPoint.y > _screenHeight && moveComponent.direction.y > 0))
            {
                moveComponent.direction = new Vector2(moveComponent.direction.x, -moveComponent.direction.y);
            }
        }

        private void RotateTowardsTarget(ref SpiderMoveComponent moveComponent, float delta)
        {
            var targetRotation = Quaternion.LookRotation(Vector3.forward, moveComponent.direction);
            moveComponent.transform.rotation = Quaternion.RotateTowards(moveComponent.transform.rotation,
                targetRotation, moveComponent.rotateSpeed * delta);
        }
    }
}