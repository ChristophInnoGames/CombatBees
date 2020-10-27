using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            float3 fieldSize = Field.size;
            Entities.WithAny<TeamATag, TeamBTag>().ForEach((ref Translation pos, ref Velocity velo) =>
            {
                float3 position = pos.Value + deltaTime * velo.Value;
                float3 velocity = velo.Value;
                if (System.Math.Abs(position.x) > fieldSize.x * .5f)
                {
                    position.x = (fieldSize.x * .5f) * Mathf.Sign(position.x);
                    velocity.x *= -.5f;
                    velocity.y *= .8f;
                    velocity.z *= .8f;
                }

                if (System.Math.Abs(position.z) > fieldSize.z * .5f)
                {
                    position.z = (fieldSize.z * .5f) * Mathf.Sign(position.z);
                    velocity.z *= -.5f;
                    velocity.x *= .8f;
                    velocity.y *= .8f;
                }

                float resourceModifier = 0f;
                if (System.Math.Abs(position.y) > fieldSize.y * .5f - resourceModifier)
                {
                    position.y = (fieldSize.y * .5f - resourceModifier) * Mathf.Sign(position.y);
                    velocity.y *= -.5f;
                    velocity.z *= .8f;
                    velocity.x *= .8f;
                }

                pos.Value = position;
                velo.Value = velocity;
            }).ScheduleParallel();
        }
    }
}