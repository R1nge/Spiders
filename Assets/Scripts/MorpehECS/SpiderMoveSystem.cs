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

            var initJob = new SpiderInitJobEcs
            {
                entities = _moveFilter,
                moveComponents = _spiderMoveStash.AsNative()
            };

            World.JobHandle = initJob.Schedule(_moveFilter.length, 64, World.JobHandle);
        }

        public override void OnUpdate(float deltaTime)
        {
            var moveJob = new SpiderJobECS
            {
                Entities = _moveFilter,
                MoveComponents = _spiderMoveStash.AsNative(),
                ScreenWidth = _screenWidth,
                ScreenHeight = _screenHeight,
                ScreenPoint = Vector2.zero,
                DeltaTime = deltaTime
            };

            World.JobHandle = moveJob.Schedule(_moveFilter.length, 64, World.JobHandle);
        }
    }
}