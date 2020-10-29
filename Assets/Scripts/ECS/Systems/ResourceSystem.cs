using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
    public class ResourceSystem : SystemBase
    {
        Vector3 minGridPos;
        Vector3 gridSize;
        Vector2Int gridCounts;

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            float3 fieldSize = Field.size;
            
            ResourceManagerECS resourceManager = ResourceManagerECS.instance;
            minGridPos = resourceManager.minGridPos;
            gridSize = resourceManager.gridSize;
            gridCounts = resourceManager.gridCounts;
            
            float snapStiffness = ResourceManagerECS.instance.snapStiffness;

            int gridX = 0;
            int gridY = 0;
            
            Entities.WithAny<ResourceTag>().WithoutBurst().ForEach((ref Translation pos, ref Velocity velo) =>
            {
                pos.Value = Vector3.Lerp(pos.Value, NearestSnappedPos(ref pos), snapStiffness * deltaTime);
                velo.Value.y += Field.gravity * deltaTime;
                pos.Value += velo.Value * deltaTime;

                GetGridIndex(ref pos, out gridX, out gridY);
                float floorY = 0f;//GetStackPos(gridX, gridY, stackHeights[resource.gridX, resource.gridY]).y;
                
                for (int j = 0; j < 3; j++)
                {
                    if (System.Math.Abs(pos.Value[j]) > Field.size[j] * .5f)
                    {
                        pos.Value[j] = Field.size[j] * .5f * Mathf.Sign(pos.Value[j]);
                        velo.Value[j] *= -.5f;
                        velo.Value[(j + 1) % 3] *= .8f;
                        velo.Value[(j + 2) % 3] *= .8f;
                    }
                }

                float3 position = pos.Value + deltaTime * velo.Value;
                float3 velocity = velo.Value;
                if (System.Math.Abs(position.x) > fieldSize.x * .5f)
                {
                    position.x = (fieldSize.x * .5f) * Mathf.Sign(position.x);
                    velocity.x *= -.5f;
                    velocity.y *= .8f;
                    velocity.z *= .8f;
                }

                if (System.Math.Abs(position.z) > fieldSize.z * .5f)
                {
                    position.z = (fieldSize.z * .5f) * Mathf.Sign(position.z);
                    velocity.z *= -.5f;
                    velocity.x *= .8f;
                    velocity.y *= .8f;
                }

                float resourceModifier = 0f;
                if (System.Math.Abs(position.y) > fieldSize.y * .5f - resourceModifier)
                {
                    position.y = (fieldSize.y * .5f - resourceModifier) * Mathf.Sign(position.y);
                    velocity.y *= -.5f;
                    velocity.z *= .8f;
                    velocity.x *= .8f;
                }

                pos.Value = position;
                velo.Value = velocity;
            }).Run();
        }
        
        
        private void GetGridIndex(ref Translation pos, out int gridX, out int gridY)
        {
            gridX = Mathf.FloorToInt((pos.Value.x - minGridPos.x + gridSize.x * .5f) / gridSize.x);
            gridY = Mathf.FloorToInt((pos.Value.z - minGridPos.y + gridSize.y * .5f) / gridSize.y);
            gridX = Mathf.Clamp(gridX, 0, gridCounts.x - 1);
            gridY = Mathf.Clamp(gridY, 0, gridCounts.y - 1);
        }

        private Vector3 NearestSnappedPos(ref Translation pos)
        {
            int x, y;
            GetGridIndex(ref pos, out x, out y);
            return new Vector3(minGridPos.x + x * gridSize.x, pos.Value.y, minGridPos.y + y * gridSize.y);
        }
        
        private Vector3 GetStackPos(int x, int y, int height) {
            return new Vector3(minGridPos.x+x*gridSize.x,-Field.size.y*.5f+(height+.5f)*1/*resourceSize*/,minGridPos.y+y*gridSize.y);
        }
    }
}