using Unity.Entities;
 using Unity.Mathematics;
 
 namespace ECS.Components
 {
     [GenerateAuthoringComponent]
     public struct Velocity : IComponentData
     {
         public float3 Value;
     }
 }