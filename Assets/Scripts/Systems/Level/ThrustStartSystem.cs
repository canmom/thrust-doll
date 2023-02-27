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

                float thrustForce =
                    SystemAPI
                        .GetSingleton<Level>()
                        .ThrustForce;

                EntityCommandBuffer.ParallelWriter ecb =
                    SystemAPI
                        .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                        .CreateCommandBuffer(state.WorldUnmanaged)
                        .AsParallelWriter();

                new ThrustStartJob
                    { CameraRotation = rotation
                    , ECB = ecb
                    , Time = SystemAPI.Time.ElapsedTime
                    , ThrustForce = thrustForce
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
    public double Time;
    public float ThrustForce;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity player, in Rotation rotation)
    {
        float3 acceleration = math.mul(CameraRotation,new float3(0f, 0f, ThrustForce ));

        ECB.AddComponent
            ( chunkIndex
            , player
            , new Thrust
                { TimeCreated = Time
                , Acceleration = acceleration
                }
            );

        ECB.AddComponent
            ( chunkIndex
            , player
            , new ThrustWindup
                { TimeCreated = Time
                , InitialRotation = rotation.Value
                , TargetRotation = CameraRotation
                }
            );

        ECB.AddComponent
            ( chunkIndex
            , player
            , new ThrustCooldown
                { TimeCreated = Time
                , InverseDuration = 0.2
                }
            );

        //target.Target = math.normalize(acceleration);
    }
}