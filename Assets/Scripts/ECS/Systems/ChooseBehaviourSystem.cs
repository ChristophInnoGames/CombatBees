using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.Systems
{
    public class ChooseBehaviourSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var aggression = BeeManagerECS.instance.aggression;
            Entities
                .WithAny<TeamATag, TeamBTag>()
                .WithNone<AttackBehaviourTag, GatherBehaviourTag, IsDeadTag>()
                .WithStructuralChanges()
                .ForEach(
                    (ref Entity entity) =>
                    {
                        if (Random.value < aggression)
                        {
                            EntityManager.AddComponentData(entity, new AttackBehaviourTag());
                        }
                        else
                        {
                            EntityManager.AddComponentData(entity, new GatherBehaviourTag());
                        }
                    }).Run();
        }
    }
}