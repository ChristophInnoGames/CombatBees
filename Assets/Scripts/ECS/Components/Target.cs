using Unity.Entities;

namespace ECS.Components
{
    [GenerateAuthoringComponent]
    public struct Target : IComponentData
    {
        public Entity Value;
    }
}