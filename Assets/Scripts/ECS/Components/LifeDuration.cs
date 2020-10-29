using Unity.Entities;
 using Unity.Mathematics;
 
 namespace ECS.Components
 {
     [GenerateAuthoringComponent]
     public struct LifeDuration : IComponentData
     {
         public float Value;
     }
 }