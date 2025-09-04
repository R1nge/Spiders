using Scellecs.Morpeh;
using Scellecs.Morpeh.Native;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using Unity.Jobs;
using UnityEngine;

namespace MorpehECS
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(SpiderMoveSystem))]
    public class SpiderMoveSystem : UpdateSystem
    {
        private NativeFilter _moveFilter;
        private Stash<SpiderMoveComponent> _spiderMoveStash;

        private Camera _camera;
        private float _screenWidth, _screenHeight;

        public override void OnAwake()
        {
            ComponentId<SpiderMoveComponent>.StashSize = Entrypoint.Instance.SpidersCount;
            
            _moveFilter = World.Filter.With<SpiderMoveComponent>().Build().AsNative();
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
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            var parallelJob = new SpiderJobECS {
                entities = _moveFilter,
                moveComponent = _spiderMoveStash.AsNative(),
            };
            
            World.JobHandle = parallelJob.Schedule(_moveFilter.length, 64, World.JobHandle);
        }
    }
}