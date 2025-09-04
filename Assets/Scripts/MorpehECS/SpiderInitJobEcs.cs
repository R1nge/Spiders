using Scellecs.Morpeh.Native;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace MorpehECS
{
    public struct SpiderInitJobEcs : IJobParallelFor
    {
        [ReadOnly] public NativeFilter entities;
        public NativeStash<SpiderMoveComponent> moveComponents;


        public void Execute(int index)
        {
            ref var spiderMoveComponent = ref moveComponents.Get(entities[index]);
            spiderMoveComponent.rotateSpeed = Entrypoint.Instance.Random.NextFloat(90f, 180f);
            spiderMoveComponent.direction = Vector2.up;
        }
    }
}