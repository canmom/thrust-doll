using Unity.Entities;
using Unity.Burst;
using Latios;

[BurstCompile]
partial struct ThrustSequenceSystem : ISystem
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

        new ThrustRotationEndJob
            { Time = SystemAPI.Time.ElapsedTime
            , RotationDuration = level.ThrustWindup
            , TransitionDuration = turnSmallToActiveTransition
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .AsParallelWriter()
            }
            .Schedule();

        new ThrustFlipEndJob
            { Time = SystemAPI.Time.ElapsedTime
            , ReverseDuration = level.ThrustWindup
            , TransitionDuration = turnSmallToActiveTransition
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .AsParallelWriter()
            }
            .Schedule();

        new ThrustActiveEndJob
            { Time = SystemAPI.Time.ElapsedTime
            , ActiveThrustDuration = level.ThrustDuration
            , TransitionDuration = level.AfterThrustTransition
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .AsParallelWriter()
            }
            .Schedule();
    }
}

partial struct ThrustRotationEndJob : IJobEntity
{
    public double Time;
    public float RotationDuration;
    public float TransitionDuration;

    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute
        ( in ThrustRotation rotation
        , Entity entity
        , [ChunkIndexInQuery] int chunkIndex
        )
    {
        if (((float) (Time - rotation.TimeCreated)) > RotationDuration)
        {
            ECB.RemoveComponent<ThrustRotation>(chunkIndex, entity);
            if (rotation.BeforeActive) {
                ECB.AddComponent
                    ( chunkIndex
                    , entity
                    , new ThrustActive
                        { TimeCreated = Time
                        }
                    );
            }
            ECB.AddComponent
                ( chunkIndex
                , entity
                , new AnimationTransition
                    { NextIndex = AnimationClipIndex.Thrust
                    , Start = (float) Time
                    , Duration = TransitionDuration
                    , Looping = true
                    }
                );
        }
    }
}

partial struct ThrustFlipEndJob : IJobEntity
{
    public double Time;
    public float ReverseDuration;
    public float TransitionDuration;

    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute
        ( in ThrustFlip flip
        , ref Drag drag
        , Entity entity
        , [ChunkIndexInQuery] int chunkIndex
        )
    {
        if (((float) (Time - flip.TimeCreated)) > ReverseDuration)
        {
            ECB.RemoveComponent<ThrustFlip>(chunkIndex, entity);
            ECB.AddComponent<ThrustRotation>
                ( chunkIndex
                , entity
                , new ThrustRotation
                    { TimeCreated = Time
                    , InitialRotation = flip.BackRotation
                    , TargetRotation = flip.TargetRotation
                    , BeforeActive = false
                    }
                );
            ECB.AddComponent
                ( chunkIndex
                , entity
                , new ThrustActive
                    { TimeCreated = Time
                    }
                );

            drag.Coefficient = flip.PreviousDrag;
        }
    }
}

partial struct ThrustActiveEndJob : IJobEntity
{
    public double Time;
    public double ActiveThrustDuration;
    public float TransitionDuration;

    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute
        ( in ThrustActive active
        , Entity entity
        , [ChunkIndexInQuery] int chunkIndex
        )
    {
        if (((float) (Time - active.TimeCreated)) > ActiveThrustDuration)
        {
            ECB.RemoveComponent<ThrustActive>(chunkIndex, entity);
            ECB.AddComponent
                ( chunkIndex
                , entity
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