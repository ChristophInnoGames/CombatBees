using ECS.Components;
using Unity.Entities;

namespace ECS.Systems
{
    public class ParticleLifeTimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.WithAny<ParticleTag>().ForEach(
	            (ref Life life, in LifeDuration lifeDuration) =>
            {
	            life.Value -= deltaTime / lifeDuration.Value;
            }).ScheduleParallel();
        }
    }
}