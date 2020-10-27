using Unity.Entities;
 using Unity.Mathematics;
 
 namespace ECS.Components
 {
     [GenerateAuthoringComponent]
     public struct SmoothDirection : IComponentData
     {
         public float3 Value;
     }
 }