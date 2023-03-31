using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Latios;

[BurstCompile]
partial struct SequenceSystem : ISystem
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

        float turnSmallToActiveTransition =
            level.TurnSmallDuration - level.ThrustWindup;

        var ecbSystem =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new ThrustWindupEndJob
            { Time = SystemAPI.Time.ElapsedTime
            , WindupDuration = level.ThrustWindup
            , TransitionDuration = turnSmallToActiveTransition
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
            }
            .Schedule();

        new ThrustActiveEndJob
            { Time = SystemAPI.Time.ElapsedTime
            , ActiveThrustDuration = level.ThrustDuration
            , TransitionDuration = level.AfterThrustTransition
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
            }
            .Schedule();

        new FlipEndJob
            { Time = SystemAPI.Time.ElapsedTime
            , ReverseDuration = level.ThrustWindup
            , TransitionDuration = turnSmallToActiveTransition
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
            }
            .Schedule();
    }
}

partial struct ThrustWindupEndJob : IJobEntity
{
    public double Time;
    public float WindupDuration;
    public float TransitionDuration;

    public EntityCommandBuffer ECB;

    void Execute
        ( in ThrustWindup windup
        , ref Drag drag
        , Entity entity
        )
    {
        if (((float) (Time - windup.TimeCreated)) > WindupDuration)
        {
            ECB.RemoveComponent<ThrustWindup>(entity);
            
            ECB.AddComponent
                ( entity
                , new ThrustActive
                    { TimeCreated = Time
                    }
                );

            ECB.AddComponent
                ( entity
                , new AnimationTransition
                    { NextIndex = AnimationClipIndex.Thrust
                    , Start = (float) Time
                    , Duration = TransitionDuration
                    , Looping = true
                    }
                );

            drag.Coefficient = windup.PreviousDrag;
        }
    }
}

partial struct FlipEndJob : IJobEntity
{
    public double Time;
    public float ReverseDuration;
    public float TransitionDuration;

    public EntityCommandBuffer ECB;

    void Execute
        ( in Flip flip
        , ref Drag drag
        , Entity entity
        )
    {
        if (((float) (Time - flip.TimeCreated)) > ReverseDuration)
        {
            ECB.RemoveComponent<Flip>(entity);
            ECB.AddComponent<RotateTo>
                ( entity
                , new RotateTo
                    { TimeCreated = Time
                    , InitialRotation = flip.BackRotation
                    , TargetRotation = flip.TargetRotation
                    , Duration = ReverseDuration
                    }
                );
        }
    }
}

partial struct ThrustActiveEndJob : IJobEntity
{
    public double Time;
    public double ActiveThrustDuration;
    public float TransitionDuration;

    public EntityCommandBuffer ECB;

    void Execute
        ( in ThrustActive active
        , Entity entity
        )
    {
        if (((float) (Time - active.TimeCreated)) > ActiveThrustDuration)
        {
            ECB.RemoveComponent<ThrustActive>(entity);
            ECB.AddComponent
                ( entity
                , new AnimationTransition
                    { NextIndex = AnimationClipIndex.LevelFlight
                    , Start = (float) Time
                    , Duration = TransitionDuration
                    , Looping = true
                    }
                );
        }
    }
}