using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class ParticleMovementSystem : SystemBase
    {
	    private EndSimulationEntityCommandBufferSystem bufferSystem;

	    protected override void OnCreate()
	    {
		    bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	    }
        protected override void OnUpdate()
        {
	        var buffer = bufferSystem.CreateCommandBuffer().ToConcurrent();
	        
            float deltaTime = Time.DeltaTime;
            float3 fieldSize = Field.size;
            var gravity = Field.gravity;
            float3 up = Vector3.up;
            Entities.WithAny<ParticleTag>().WithNone<IsStuckTag>().ForEach(
	            (Entity entity, int entityInQueryIndex, ref Translation pos, ref Velocity velo, ref NonUniformScale nonUniformScale) =>
            {
	            float3 velocity = velo.Value + up * (gravity * deltaTime);;
	            float3 position = pos.Value + deltaTime * velocity;
	            float3 scale = nonUniformScale.Value;
	            bool stuck = false;

                if (System.Math.Abs(position.x) > fieldSize.x * .5f) {
                	position.x = fieldSize.x * .5f * Mathf.Sign(position.x);
                	float splat = Mathf.Abs(velocity.x*.3f) + 1f;
                    scale.y *= splat;
                    scale.z *= splat;
                	stuck = true;
                }
                if (System.Math.Abs(position.y) > fieldSize.y * .5f) {
                	position.y = fieldSize.y * .5f * Mathf.Sign(position.y);
                	float splat = Mathf.Abs(velocity.y * .3f) + 1f;
                    scale.z *= splat;
                    scale.x *= splat;
                	stuck = true;
                }

                if (System.Math.Abs(position.z) > fieldSize.z * .5f)
                {
	                position.z = fieldSize.z * .5f * Mathf.Sign(position.z);
	                float splat = Mathf.Abs(velocity.z * .3f) + 1f;
	                scale.x *= splat;
	                scale.y *= splat;
	                stuck = true;
                }

                pos.Value = position;
                velo.Value = velocity;
                nonUniformScale.Value = scale;

                if (stuck)
                {
	                buffer.AddComponent(entityInQueryIndex, entity, new IsStuckTag());
                }
            }).ScheduleParallel();
            bufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}