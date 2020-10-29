using ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems
{
    public class BoidsSystem : SystemBase
    {
        private EntityQuery teamAQuery;
        private EntityQuery teamBQuery;
        private Random random;

        protected override void OnCreate()
        {
            base.OnCreate();
            teamAQuery = GetEntityQuery(typeof(TeamATag), ComponentType.ReadOnly<Translation>(), ComponentType.Exclude<IsDeadTag>());
            teamBQuery = GetEntityQuery(typeof(TeamBTag), ComponentType.ReadOnly<Translation>(), ComponentType.Exclude<IsDeadTag>());
            random = new Random();
            random.InitState();
        }

        protected override void OnUpdate()
        {
            var teamA = teamAQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var teamB = teamBQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            bool teamAAlive = teamA.Length > 0;
            bool teamBAlive = teamB.Length > 0;

            float deltaTime = Time.DeltaTime;
            float teamAttraction = BeeManagerECS.instance.teamAttraction;
            float teamRepulsion = BeeManagerECS.instance.teamRepulsion;
            float3 maxJitter = new float3(BeeManagerECS.instance.flightJitter * deltaTime);
            float damping = BeeManagerECS.instance.damping;

            var random = this.random;

            Entities
                .WithAny<TeamATag, TeamBTag>()
                .WithNone<Target, IsDeadTag>()
                .WithReadOnly(teamA)
                .WithReadOnly(teamB)
                .WithDeallocateOnJobCompletion(teamA)
                .WithDeallocateOnJobCompletion(teamB)
                .ForEach(
                    (ref Entity entity, ref Velocity velocity, in Translation pos) =>
                    {
                        float3 friendPosition = float3.zero;
                        float3 enemyPosition = float3.zero;
                        if (HasComponent<TeamATag>(entity))
                        {
                            if (teamAAlive)
                                friendPosition = teamA[random.NextInt(0, teamA.Length)].Value;
                            if (teamBAlive)
                                enemyPosition = teamB[random.NextInt(0, teamB.Length)].Value;
                        }
                        else
                        {
                            if (teamBAlive)
                                friendPosition = teamB[random.NextInt(0, teamB.Length)].Value;
                            if (teamAAlive)
                                enemyPosition = teamA[random.NextInt(0, teamA.Length)].Value;
                        }

                        float3 velo = velocity.Value + random.NextFloat3(float3.zero, maxJitter);
                        velo *= (1f - damping);


                        float3 delta = friendPosition - pos.Value;
                        float dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                        if (dist > 0f)
                        {
                            velo += delta * (teamAttraction * deltaTime / dist);
                        }

                        delta = enemyPosition - pos.Value;
                        dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                        if (dist > 0f)
                        {
                            velo -= delta * (teamRepulsion * deltaTime / dist);
                        }

                        velocity.Value = velo;
                    }).ScheduleParallel();
        }
    }
}