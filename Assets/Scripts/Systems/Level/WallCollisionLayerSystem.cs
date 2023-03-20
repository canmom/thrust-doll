using Unity.Entities;
using Unity.Burst;
using Latios;
using Latios.Psyshock;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(VelocitySystem))]
partial struct WallCollisionLayerSystem : ISystem, ISystemNewScene
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
                ( new WallCollisionLayer()
                );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _handles.Update(ref state);

        var settings = BuildCollisionLayerConfig.defaultSettings;

        EntityQuery wallQuery =
            SystemAPI
                .QueryBuilder()
                .WithAll<WallCollider, Collider>()
                .Build();

        state.Dependency =
            Physics
                .BuildCollisionLayer(wallQuery, _handles)
                .WithSettings(settings)
                .ScheduleParallel
                    ( out CollisionLayer wallLayer
                    , Allocator.Persistent
                    , state.Dependency
                    );

        var wallLayerComponent =
            new WallCollisionLayer
                { Layer = wallLayer
                };

        _latiosWorld
            .sceneBlackboardEntity
            .SetCollectionComponentAndDisposeOld
                ( wallLayerComponent
                );

        //state.Enabled = false;
    }
}