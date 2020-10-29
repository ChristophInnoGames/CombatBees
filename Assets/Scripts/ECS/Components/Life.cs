using Unity.Entities;
 using Unity.Mathematics;
 
 namespace ECS.Components
 {
     [GenerateAuthoringComponent]
     public struct Life : IComponentData
     {
         public float Value;
     }
 }