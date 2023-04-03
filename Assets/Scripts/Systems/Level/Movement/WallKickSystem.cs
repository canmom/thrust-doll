using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Latios;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(SDFCollisionSystem))]
partial struct WallKickSystem : ISystem
{
    LatiosWorldUnmanaged _latiosWorld;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _latiosWorld = state.GetLatiosWorldUnmanaged();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Level level =
            _latiosWorld
                .sceneBlackboardEntity
                .GetComponentData<Level>();

        var ecbSystem =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new FaceWallStartJob
            { Time = SystemAPI.Time.ElapsedTime
            , ECB = 
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
            , FacingDuration = 
                level
                    .WallkickFacingDuration
            }
            .Schedule();

        new WallKickLerpJob
            { Time = SystemAPI.Time.ElapsedTime
            , NotionalDuration =
                level
                    .WallkickFacingDuration
            , TransientDuration =
                level
                    .WallkickTransientDuration
            , StopDuration =
                level
                    .WallkickStopDuration
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
            }
            .Schedule();

        new FaceWallEndJob
            { Time = SystemAPI.Time.ElapsedTime
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
            , RealignmentDuration =
                level
                    .WallkickRealignmentDuration
            }
            .Schedule();

        new WallkickEndJob
            { Time = SystemAPI.Time.ElapsedTime
            , WallkickStopDuration =
                level
                    .WallkickStopDuration
            , TransitionDuration =
                level
                    .WallkickFacingDuration
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
            }
            .Schedule();
    }
}

[WithAll(typeof(Character))]
[WithNone(typeof(WallKick),typeof(FaceWall),typeof(Flip))]
[BurstCompile]
partial struct FaceWallStartJob : IJobEntity
{
    public double Time;
    public EntityCommandBuffer ECB;
    public float FacingDuration;

    void Execute
        ( Entity entity
        , ref Velocity velocity
        , in Rotation rotation
        , in SDFCollision collision
        , ref DynamicAnimationClip dynamicAnimation
        )
    {
        //set up the dynamic animation
        dynamicAnimation =
            new DynamicAnimationClip
                { Index = AnimationClipIndex.WallKickShallow
                , SampleTime = 0
                , Weight = 0
                };

        //end any thrust early
        ECB.RemoveComponent<Flip>(entity);
        ECB.RemoveComponent<RotateTo>(entity);

        ECB.AddComponent
            ( entity
            , new FaceWall
                { InitialRotation = rotation.Value
                , TimeCreated = Time
                , NoCollision = false
                }
            );


    }
}

[BurstCompile]
partial struct WallKickLerpJob : IJobEntity
{
    public double Time;

    public float NotionalDuration;
    public float TransientDuration;
    public float StopDuration;

    public EntityCommandBuffer ECB;

    void Execute
        ( in SDFCollision collision
        , in CollidesWithSDF collider
        , ref FaceWall faceWall
        , ref Velocity velocity
        , ref Rotation rotation
        , ref DynamicAnimationClip dynamicAnimation
        , ref CurrentAnimationClip clip
        , Entity entity
        )
    {
        float tau =
            1f
            - (collision.Distance - collider.InnerRadius)
            / (   collider.Radius - collider.InnerRadius);

        float transient =
            math.smoothstep
                (0
                , TransientDuration
                , (float) (Time - faceWall.TimeCreated)
                );

        quaternion targetRotation =
            quaternion
                .LookRotationSafe
                    ( velocity.Value
                    , collision.Normal
                    );

        rotation.Value =
            math
                .slerp
                    ( faceWall.InitialRotation
                    , targetRotation
                    , math.clamp( tau, 0, 1)
                    );

        dynamicAnimation.SampleTime =
            tau * NotionalDuration;

        dynamicAnimation.Weight =
            math.clamp(tau, 0, 1) * transient;

        //inner collider
        if (tau > 1f && !faceWall.NoCollision) {
            float NotionalStartTime = (float) (Time - tau * NotionalDuration);

            clip =
                new CurrentAnimationClip
                    { Index = AnimationClipIndex.WallKickShallow
                    , Start = NotionalStartTime
                    , Looping = false
                    };

            dynamicAnimation.Weight = 0;

            float3 targetVelocity =
                velocity.Value - 2 * math.dot(velocity.Value, collision.Normal) * collision.Normal;

            quaternion finalTargetRotation =
                quaternion
                    .LookRotation
                        ( math.normalize(targetVelocity)
                        , collision.Normal
                        );

            ComponentTypeSet toRemove =
                new ComponentTypeSet
                    ( ComponentType.ReadWrite<Thrust>()
                    , ComponentType.ReadWrite<ThrustActive>()
                    , ComponentType.ReadWrite<FaceWall>()
                    );

            ECB.RemoveComponent(entity, toRemove);
            ECB.AddComponent
                ( entity
                , new RotateTo
                    { InitialRotation = rotation.Value
                    , TargetRotation = finalTargetRotation
                    , TimeCreated = Time
                    , Duration = StopDuration
                    }
                );
            // rotation.Value = finalTargetRotation;
            ECB.AddComponent
                ( entity
                , new WallKick
                    { IncidentVelocity = velocity.Value
                    , ReflectionVelocity = targetVelocity
                    , Normal = collision.Normal
                    , TimeCreated = Time
                    }
                );

            velocity.Value = new float3(0f);
        }

        if (tau < 1f) {
            faceWall.NoCollision = false;
        }
    }
}

[WithNone(typeof(SDFCollision))]
[BurstCompile]
partial struct FaceWallEndJob : IJobEntity
{
    public double Time;

    public float RealignmentDuration;

    public EntityCommandBuffer ECB;

    void Execute
        ( Entity entity
        , in Rotation rotation
        , in FaceWall faceWall
        , in Velocity velocity
        , ref DynamicAnimationClip dynamicClip
        )
    {
        ECB.RemoveComponent<FaceWall>(entity);
        ECB.AddComponent<RotateTo>
            ( entity
            , new RotateTo
                { InitialRotation = rotation.Value
                , TargetRotation =
                    quaternion.LookRotationSafe
                        ( velocity.Value
                        , new float3 (0, 1, 0)
                        )
                , TimeCreated = Time
                , Duration = RealignmentDuration
                }
            );

        dynamicClip.Weight = 0;
    }
}

[BurstCompile]
partial struct WallkickEndJob : IJobEntity
{
    public double Time;
    public double WallkickStopDuration;
    public float TransitionDuration;

    public EntityCommandBuffer ECB;

    void Execute
        ( in WallKick wallkick
        , in Rotation rotation
        , ref Velocity velocity
        , in SDFCollision collision
        , Entity entity
        )
    {
        if (((float) (Time - wallkick.TimeCreated)) > WallkickStopDuration)
        {
            velocity.Value = wallkick.ReflectionVelocity;
            ECB.RemoveComponent<WallKick>(entity);
            ECB.AddComponent
                ( entity
                , new AnimationTransition
                    { NextIndex = AnimationClipIndex.LevelFlight
                    , Start = (float) Time
                    , Duration = TransitionDuration
                    , Looping = true
                    }
                );
            ECB.AddComponent
                ( entity
                , new FaceWall
                    { InitialRotation =
                        quaternion
                            .LookRotationSafe
                                ( wallkick.ReflectionVelocity
                                , collision.Normal
                                )
                    , TimeCreated = Time
                    , NoCollision = true
                    }
                );
        }
    }
}