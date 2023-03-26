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

                EntityCommandBuffer ecb =
                    SystemAPI
                        .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                        .CreateCommandBuffer(state.WorldUnmanaged);

                new ThrustStartJob
                    { CameraRotation = rotation
                    , ECB = ecb
                    , Time = SystemAPI.Time.ElapsedTime
                    , ThrustForce = level.ThrustForce
                    , InverseThrustCooldown = 1f/level.ThrustCooldown
                    , TurnSmallTransitionIn = level.TurnSmallTransitionIn
                    , IncreasedDrag = level.IncreasedDragDuringFlip
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
    public EntityCommandBuffer ECB;
    public double Time;
    public float ThrustForce;
    public float InverseThrustCooldown;
    public float TurnSmallTransitionIn;
    public float IncreasedDrag;

    void Execute
        ( Entity player
        , in Rotation rotation
        , ref Drag drag
        )
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
            math.mul(toLocalSpace, math.normalize(acceleration));

        ECB.AddComponent
            ( player
            , new Thrust
                { TimeCreated = Time
                , Acceleration = acceleration
                }
            );

        ECB.AddComponent
            ( player
            , new ThrustWindup
                { TimeCreated = Time
                , PreviousDrag = drag.Coefficient
                }
            );

        if ( accelLocal.z > 0.5) {    
            AnimationClipIndex clipToPlay =
                math.abs(accelLocal.x) > math.abs(accelLocal.y)
                    ? accelLocal.x > 0
                        ? AnimationClipIndex.TurnSmallRight
                        : AnimationClipIndex.TurnSmallLeft
                    : accelLocal.y > 0
                        ? AnimationClipIndex.TurnSmallUp
                        : AnimationClipIndex.TurnSmallDown;

            ECB.AddComponent
                (  player
                , new RotateTo
                    { TimeCreated = Time
                    , InitialRotation = rotation.Value
                    , TargetRotation = CameraRotation
                    }
                );

            ECB.AddComponent
                (  player
                , new AnimationTransition
                    { NextIndex = clipToPlay
                    , Start = (float) Time
                    , Duration = TurnSmallTransitionIn
                    , Looping = false
                    }
                );
        } else {
            ECB.AddComponent
                (  player
                , new Flip
                    { TimeCreated = Time
                    , InitialRotation = rotation.Value
                    , BackRotation =
                        quaternion
                            .LookRotationSafe
                                ( acceleration
                                , math.mul(rotation.Value, new float3 (0, -1, 0))
                                )
                    , TargetRotation = CameraRotation
                    }
                );

            ECB.AddComponent
                (  player
                , new AnimationTransition
                    { NextIndex = AnimationClipIndex.TurnReverse 
                    , Start = (float) Time
                    , Duration = TurnSmallTransitionIn
                    , Looping = false
                    }
                );

            drag.Coefficient = IncreasedDrag;
        }

        ECB.AddComponent
            (  player
            , new ThrustCooldown
                { TimeCreated = Time
                , InverseDuration = InverseThrustCooldown
                }
            );
    }
}