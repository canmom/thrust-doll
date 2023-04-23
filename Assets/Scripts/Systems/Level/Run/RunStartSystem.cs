using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Latios;
using Latios.Systems;

[UpdateInGroup(typeof(PreSyncPointGroup))]
[BurstCompile]
partial struct RunStartSystem : ISystem
{
    LatiosWorldUnmanaged _latiosWorld;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _latiosWorld = state.GetLatiosWorldUnmanaged();

        state.RequireForUpdate<StartNewRun>();
        state.RequireForUpdate<SpawnPoint>();
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
            .AddComponentDataIfMissing
                ( new LevelStats
                    { Deaths = 0
                    , BestTime = 1.0/0.0
                    }
                );

        EntityCommandBuffer ecb =
            _latiosWorld
                .syncPoint
                .CreateEntityCommandBuffer();

        ecb.RemoveComponent<StartNewRun>(blackboard);

        ecb.AddComponent
            ( blackboard
            , new Run
                { Start = SystemAPI.Time.ElapsedTime
                }
            );

        Translation spawnPoint =
            SystemAPI
                .GetComponent<Translation>
                    ( SystemAPI
                        .GetSingletonEntity<SpawnPoint>()
                    );

        Entity dollPrefab =
            blackboard
                .GetComponentData<Level>()
                .DollPrefab;

        _latiosWorld
            .syncPoint
            .CreateInstantiateCommandBuffer<Translation>()
            .Add(dollPrefab, spawnPoint);
    }
}