using Unity.Entities;
using Unity.Burst;
using Latios;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
partial struct RunStartSystem : ISystem
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
        BlackboardEntity blackboard =
            _latiosWorld
                .sceneBlackboardEntity;

        blackboard
            .AddComponentData
                ( new Run
                    { Start = SystemAPI.Time.DeltaTime
                    }
                );

        blackboard
            .AddComponentDataIfMissing
                ( new LevelStats
                    { Deaths = 0
                    , BestTime = 1.0/0.0
                    }
                );

        state.Enabled = false;
    }
}