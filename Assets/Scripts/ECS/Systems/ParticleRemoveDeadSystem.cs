using ECS.Components;
using Unity.Entities;

namespace ECS.Systems
{
    public class ParticleRemoveDeadSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _bufferSystem;

        protected override void OnCreate()
        {
            _bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var buffer = _bufferSystem.CreateCommandBuffer();

            Entities.WithoutBurst().ForEach((Entity entity, in Life life) =>
            {
                if (life.Value > 0f)
                {
                    return;
                }
            
                buffer.DestroyEntity(entity);
            }).Run();
        }
    }
}