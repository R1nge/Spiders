using Scellecs.Morpeh.Native;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace MorpehECS
{
    public struct SpiderJobECS : IJobParallelFor
    {
        [ReadOnly] public NativeFilter Entities;
        public NativeStash<SpiderMoveComponent> MoveComponents;
        public float ScreenWidth, ScreenHeight;
        public Vector2 ScreenPoint;
        public float DeltaTime;

        public void Execute(int index)
        {
            var entity = Entities[index];

            ref var move = ref MoveComponents.Get(entity);
            TickTimer(ref move, DeltaTime);
            ChangeDirection(ref move);
            HandleOffScreen(ref move, ScreenPoint);
            RotateTowardsTarget(ref move, DeltaTime);
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
            }
        }

        private void HandleOffScreen(ref SpiderMoveComponent moveComponent, Vector3 screenPoint)
        {
            if (screenPoint.x < 0 && moveComponent.direction.x < 0 ||
                (screenPoint.x > ScreenWidth && moveComponent.direction.x > 0))
            {
                moveComponent.direction = new Vector2(-moveComponent.direction.x, moveComponent.direction.y);
            }

            if (screenPoint.y < 0 && moveComponent.direction.y < 0 ||
                (screenPoint.y > ScreenHeight && moveComponent.direction.y > 0))
            {
                moveComponent.direction = new Vector2(moveComponent.direction.x, -moveComponent.direction.y);
            }
        }

        private void RotateTowardsTarget(ref SpiderMoveComponent moveComponent, float delta)
        {
            var targetRotation = Quaternion.LookRotation(Vector3.forward, moveComponent.direction);
            moveComponent.rotation = Quaternion.RotateTowards(moveComponent.rotation, targetRotation, moveComponent.rotateSpeed * delta);
        }
    }
}