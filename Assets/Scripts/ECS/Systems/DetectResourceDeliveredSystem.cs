using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class DetectResourceDeliveredSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem bufferSystem;
        
        protected override void OnCreate()
        {
            bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var buffer = bufferSystem.CreateCommandBuffer().ToConcurrent();
            float sizeX = Field.size.x;
            float floorY = -9.7f;
            
            Entities.WithAll<ResourceTag>().ForEach((Entity entity, int entityInQueryIndex, ref Translation pos) =>
            {
                if (pos.Value.y < floorY) 
                {
                    pos.Value.y = floorY;
                    if (Mathf.Abs(pos.Value.x) > sizeX * .4f) 
                    {
                        int team = 0;
                        if (pos.Value.x > 0f) 
                        {
                            team = 1;
                        }
                        
                        for (int j = 0; j < 1; j++) 
                        {
                            BeeManagerECS.SpawnBee(pos.Value, team);
                        }
                        
                        buffer.DestroyEntity(entityInQueryIndex, entity);
                    }
                }
                
            }).ScheduleParallel();
            
            bufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}