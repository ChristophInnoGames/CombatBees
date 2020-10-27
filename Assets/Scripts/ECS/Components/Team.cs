using Unity.Entities;

namespace ECS.Components
 {
     public enum TeamEnum
     {
         A,
         B
     };
     
     [GenerateAuthoringComponent]
     public struct Team : ISharedComponentData
     {
         public TeamEnum Value;
     }
 }