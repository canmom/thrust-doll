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

                Level level =
                    SystemAPI
                        .GetSingleton<Level>();

                EntityCommandBuffer.ParallelWriter ecb =
                    SystemAPI
                        .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                        .CreateCommandBuffer(state.WorldUnmanaged)
                        .AsParallelWriter();

                new ThrustStartJob
                    { CameraRotation = rotation
                    , ECB = ecb
                    , Time = SystemAPI.Time.ElapsedTime
                    , ThrustForce = level.ThrustForce
                    , InverseThrustCooldown = 1f/level.ThrustCooldown
                    , TurnSmallTransitionIn = level.TurnSmallTransitionIn
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
    public float InverseThrustCooldown;
    public float TurnSmallTransitionIn;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity player, in Rotation rotation)
    {
        float3 acceleration =
            math.mul
                ( CameraRotation
                , new float3
                    ( 0f
                    , 0f
                    , ThrustForce
                    )
                );

        quaternion toLocalSpace = math.inverse(rotation.Value);

        float3 accelLocal =
            math.mul(toLocalSpace, acceleration);

        ECB.AddComponent
            ( chunkIndex
            , player
            , new Thrust
                { TimeCreated = Time
                , Acceleration = acceleration
                }
            );
   
        AnimationClipIndex clipToPlay =
            math.abs(accelLocal.x) > math.abs(accelLocal.y)
                ? accelLocal.x > 0
                    ? AnimationClipIndex.TurnSmallRight
                    : AnimationClipIndex.TurnSmallLeft
                : accelLocal.y > 0
                    ? AnimationClipIndex.TurnSmallUp
                    : AnimationClipIndex.TurnSmallDown;

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
            , new AnimationTransition
                { NextIndex = clipToPlay
                , Start = (float) Time
                , Duration = TurnSmallTransitionIn
                , Looping = false
                }
            );

        ECB.AddComponent
            ( chunkIndex
            , player
            , new ThrustCooldown
                { TimeCreated = Time
                , InverseDuration = InverseThrustCooldown
                }
            );

        //target.Target = math.normalize(acceleration);
    }
}