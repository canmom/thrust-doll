using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct CubeGridSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.Enabled = false;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        int numCubesX = 50;
        int numCubesZ = 50;

        var level = SystemAPI.GetSingleton<Level>();

        Entity cubePrefab = level.CubePrefab;

        var cubes = new NativeArray<Entity>(numCubesX * numCubesZ, Allocator.TempJob);

        EntityCommandBuffer ECB =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

        ECB.Instantiate(cubePrefab, cubes);

        for (int i = 0; i < numCubesX; ++i)
        {
            for (int j = 0; j < numCubesZ; ++j)
            {
                ECB.SetComponent
                    ( cubes[i*numCubesX+j]
                    , new Translation
                        { Value = new float3 (-250f+i * 10f, 0f, -250f+j * 10f)
                        }
                    );
            }
        }

        state.Enabled = false;
    }
}