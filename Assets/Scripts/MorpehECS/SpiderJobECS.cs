using Scellecs.Morpeh.Native;
using Unity.Collections;
using Unity.Jobs;

namespace MorpehECS
{
    public struct SpiderJobECS : IJobParallelFor
    {
        [ReadOnly] public NativeFilter entities;
        public NativeStash<SpiderMoveComponent> moveComponent;

        public void Execute(int index)
        {
            var entity = this.entities[index];

            ref var component = ref this.moveComponent.Get(entity);

            var screenPoint = _camera.WorldToScreenPoint(move.transform.position);
            var speed = move.moveSpeed;
            TickTimer(ref move, deltaTime);
            ChangeDirection(ref move);
            HandleOffScreen(ref move, screenPoint);
            RotateTowardsTarget(ref move, deltaTime);
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