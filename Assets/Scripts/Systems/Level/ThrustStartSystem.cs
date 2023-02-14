using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(CameraControllerSystem))]
[UpdateBefore(typeof(RotationSpringSystem))]
[BurstCompile]
partial struct ThrustStartSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Character>();
        state.RequireForUpdate<CameraPivot>();
        state.RequireForUpdate<Intent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.GetSingleton<Intent>().Thrust)
        {   
            Entity player =
                SystemAPI
                    .GetSingletonEntity<Character>();

            if (!SystemAPI.HasComponent<ThrustCooldown>(player))
            {
                quaternion rotation =
                    SystemAPI
                        .GetComponent<Rotation>
                            ( SystemAPI
                                .GetSingletonEntity<CameraPivot>()
                            )
                        .Value;

                EntityCommandBuffer.ParallelWriter ecb =
                    SystemAPI
                        .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                        .CreateCommandBuffer(state.WorldUnmanaged)
                        .AsParallelWriter();

                new ThrustStartJob
                    { CameraRotation = rotation
                    , ECB = ecb
                    }
                    .Schedule();
            }
        }
    }
}

[WithAll(typeof(Character))]
partial struct ThrustStartJob : IJobEntity
{
    public quaternion CameraRotation;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity player, ref RotationTarget target)
    {
        float3 acceleration = math.mul(CameraRotation,new float3(0f, 0f, 10f ));

        ECB.AddComponent
            ( chunkIndex
            , player
            , new Thrust
                { TimeRemaining = 0.5f
                , Acceleration = acceleration
                }
            );

        ECB.AddComponent
            ( chunkIndex
            , player
            , new ThrustCooldown
                { TimeRemaining = 5f
                , InverseDuration = 0.2f
                }
            );

        target.Target = math.normalize(acceleration);
    }
}