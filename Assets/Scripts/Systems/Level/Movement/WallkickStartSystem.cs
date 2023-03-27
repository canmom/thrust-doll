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
            , FacingDuration = 
                level.WallkickFacingDuration
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
[WithNone(typeof(WallKick),typeof(ClearingWall),typeof(FaceWall))]
partial struct WallKickStartJob : IJobEntity
{
    public double Time;
    public EntityCommandBuffer ECB;
    public float FacingDuration;
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

        // initiate the animation
        ECB.AddComponent
            ( entity
            , new AnimationTransition
                { NextIndex = AnimationClipIndex.WallKickShallow
                , Start = (float) Time
                , Duration = FacingDuration //controls amount of lerp
                , Looping = false
                }
            );

        ECB.AddComponent
            ( entity
            , new AnimationClipTimeOverride
                { ClipTime = 0f
                }
            );

        ECB.AddComponent
            ( entity
            , new FaceWall
                { InitialRotation = rotation.Value
                }
            );
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