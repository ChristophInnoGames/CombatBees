using ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
    [UpdateAfter(typeof(ChooseBehaviourSystem))]
    public class ChooseAttackTargetSystem : SystemBase
    {
        private EntityQuery teamAQuery;
        private EntityQuery teamBQuery;
        private Random random;

        protected override void OnCreate()
        {
            base.OnCreate();
            teamAQuery = GetEntityQuery(typeof(TeamATag), ComponentType.Exclude<IsDeadTag>());
            teamBQuery = GetEntityQuery(typeof(TeamBTag), ComponentType.Exclude<IsDeadTag>());
            random = new Random();
            random.InitState();
        }

        protected override void OnUpdate()
        {
            var teamA = teamAQuery.ToEntityArray(Allocator.TempJob);
            var teamB = teamBQuery.ToEntityArray(Allocator.TempJob);

            bool teamAAlive = teamA.Length > 0;
            bool teamBAlive = teamB.Length > 0;

            Entities
                .WithAny<AttackBehaviourTag>()
                .WithNone<Target, IsDeadTag>()
                .WithStructuralChanges()
                .WithReadOnly(teamA)
                .WithReadOnly(teamB)
                .WithDeallocateOnJobCompletion(teamA)
                .WithDeallocateOnJobCompletion(teamB)
                .ForEach(
                    (ref Entity entity) =>
                    {
                        if (HasComponent<TeamATag>(entity))
                        {
                            AssignTargetOrRemoveBehaviour(ref entity, teamBAlive, in teamB);
                        }
                        else
                        {
                            AssignTargetOrRemoveBehaviour(ref entity, teamAAlive, in teamA);
                        }
                    }).Run();
            
            //teamA.Dispose();
            //teamB.Dispose();
        }

        private void AssignTargetOrRemoveBehaviour(ref Entity entity, bool otherTeamAlive, in NativeArray<Entity> otherTeam)
        {
            if (otherTeamAlive)
            {
                var index = random.NextInt(0, otherTeam.Length);
                var entityTarget = otherTeam[index];
                EntityManager.AddComponentData(entity, new Target {Value = entityTarget});
            }
            else
            {
                EntityManager.RemoveComponent<AttackBehaviourTag>(entity);
            }
        }
    }
}