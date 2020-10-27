using Unity.Entities;
 using Unity.Mathematics;
 
 namespace ECS.Components
 {
     [GenerateAuthoringComponent]
     public struct SmoothPosition : IComponentData
     {
         public float3 Value;
     }
 }