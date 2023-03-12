using Unity.Entities;
using Unity.Burst;
using Latios;
using Latios.Psyshock;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(VelocitySystem))]
partial struct PlayerCollisionLayerSystem : ISystem, ISystemNewScene
{
    BuildCollisionLayerTypeHandles _handles;

    LatiosWorldUnmanaged _latiosWorld;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _handles = new BuildCollisionLayerTypeHandles(ref state);

        _latiosWorld = state.GetLatiosWorldUnmanaged();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnNewScene(ref SystemState state)
    {
        _latiosWorld
            .sceneBlackboardEntity
            .AddOrSetCollectionComponentAndDisposeOld
                ( new PlayerCollisionLayer()
                );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _handles.Update(ref state);

        var settings = BuildCollisionLayerConfig.defaultSettings;

        EntityQuery playerQuery =
            SystemAPI
                .QueryBuilder()
                .WithAll<Character, Collider>()
                .Build();

        state.Dependency =
            Physics
                .BuildCollisionLayer(playerQuery, _handles)
                .WithSettings(settings)
                .ScheduleParallel
                    ( out CollisionLayer playerLayer
                    , Allocator.Persistent
                    , state.Dependency
                    );

        var playerLayerComponent =
            new PlayerCollisionLayer
                { Layer = playerLayer
                };

        _latiosWorld
            .sceneBlackboardEntity
            .SetCollectionComponentAndDisposeOld
                ( playerLayerComponent
                );
    }
}