using Scellecs.Morpeh.Native;
using Unity.Collections;
using Unity.Jobs;

namespace MorpehECS
{
    public struct SpiderJobECS : IJobParallelFor
    {
        [ReadOnly]
        public NativeFilter entities;
        public NativeStash<SpiderMoveComponent> moveComponent;
        
        public void Execute(int index) {
            var entity = this.entities[index];
        
            ref var component = ref this.moveComponent.Get(entity);
            
            component.Value += 1;
        }
    }
}