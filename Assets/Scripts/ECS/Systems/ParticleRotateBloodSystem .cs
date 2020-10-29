using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class ParticleBloodRotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var speedStretch = ParticleManagerECS.instance.speedStretch;
            float3 up = Vector3.up;
            Entities.WithAny<BloodTag>().WithNone<IsStuckTag>().ForEach(
	            (ref Rotation rot, ref NonUniformScale nonUniformScale, in Velocity velo) =>
            {
	            float3 scale = nonUniformScale.Value;
	            rot.Value = quaternion.LookRotation(velo.Value, up); 
	            scale.z *= 1f + math.length(rot.Value) * speedStretch;
	            nonUniformScale.Value = scale;
            }).ScheduleParallel();
        }
    }
}