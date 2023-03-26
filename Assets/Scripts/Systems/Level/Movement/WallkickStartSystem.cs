using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using Latios;

[BurstCompile]
partial struct WallkickStartSystem : ISystem
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
        EntityCommandBuffer ecb1 =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

        EntityCommandBuffer ecb2 =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

        Level level =
            _latiosWorld
                .sceneBlackboardEntity
                .GetComponentData<Level>();

        new WallKickStartJob
            { Time = SystemAPI.Time.ElapsedTime
            , ECB = ecb1
            , FlipTransitionIn = 
                level.TurnSmallTransitionIn
            , TransitionLookup =
                SystemAPI.
                    GetComponentLookup<AnimationTransition>()
            }
            .Schedule();

        new ClearingWallJob
            { ECB = ecb2
            }
            .Schedule();
    }
}

[WithAll(typeof(Character))]
[WithNone(typeof(WallKick),typeof(ClearingWall))]
partial struct WallKickStartJob : IJobEntity
{
    public double Time;
    public EntityCommandBuffer ECB;
    public float FlipTransitionIn;
    public ComponentLookup<AnimationTransition> TransitionLookup;

    void Execute
        ( Entity entity
        , ref Velocity velocity
        , in Rotation rotation
        , in SDFCollision collision
        , ref CurrentAnimationClip clip
        )
    {
        // clear any animation transition
        if ( TransitionLookup.TryGetComponent(entity, out var transition) )
        {
            clip =
                new CurrentAnimationClip
                    { Index = transition.NextIndex
                    , Start = transition.Start
                    , Looping = transition.Looping
                    };
        }

        //end any thrust early
        ECB.RemoveComponent<ThrustActive>(entity);

        //create a new WallKick
        float3 reflectionVelocity =
            velocity.Value - 2 * math.dot(velocity.Value, collision.Normal) * collision.Normal;
            

        ECB.AddComponent
            ( entity
            , new WallKick
                { IncidentVelocity = velocity.Value
                , ReflectionVelocity = reflectionVelocity
                , Normal = collision.Normal
                , TimeCreated = Time
                }
            );

        // initiate the animation
        ECB.AddComponent
            ( entity
            , new AnimationTransition
                { NextIndex = AnimationClipIndex.TurnReverse 
                , Start = (float) Time
                , Duration = FlipTransitionIn
                , Looping = false
                }
            );

        // rotate the character to point along the bounced off vector
        ECB.AddComponent
            ( entity
            , new Flip
                { InitialRotation =
                    rotation.Value
                , BackRotation =
                    quaternion
                        .LookRotationSafe
                            ( reflectionVelocity
                            , math.mul(rotation.Value, new float3 (0, -1, 0))
                            )
                , TargetRotation =
                    quaternion
                        .LookRotationSafe
                            ( reflectionVelocity
                            , new float3 (0, 1, 0)
                            )
                , TimeCreated = Time
                }
            );

        //zero out the velocity
        velocity.Value = new float3(0f);
    }
}

[WithAll(typeof(Character), typeof(ClearingWall))]
[WithNone(typeof(WallKick), typeof(SDFCollision))]
partial struct ClearingWallJob : IJobEntity
{
    public EntityCommandBuffer ECB;

    void Execute
        ( Entity entity
        )
    {
        ECB.RemoveComponent<ClearingWall>(entity);
    }
}