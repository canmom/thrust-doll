using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Latios;
using Latios.Psyshock;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(ProjectileCollisionLayerSystem))]
partial struct PlayerCollisionSystem : ISystem
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

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var projectileCollisionLayer =
            _latiosWorld
                .sceneBlackboardEntity
                .GetCollectionComponent<ProjectileCollisionLayer>(true)
                .Layer;

        var wallCollisionLayer =
            _latiosWorld
                .sceneBlackboardEntity
                .GetCollectionComponent<WallCollisionLayer>(true)
                .Layer;

        new PlayerColliderCastJob
            { TargetLayer = wallCollisionLayer
            }
            .Schedule();

        var playerCollisionLayer =
            _latiosWorld
                .sceneBlackboardEntity
                .GetCollectionComponent<PlayerCollisionLayer>(true)
                .Layer;

        var processor = new DamagePlayerProcessor
        {

        };

        state.Dependency =
            Latios.Psyshock.Physics
                .FindPairs
                    ( wallCollisionLayer
                    , playerCollisionLayer
                    , processor
                    )
                    .ScheduleParallel(state.Dependency);

        //state.Dependency = PhysicsDebug.DrawLayer(wallCollisionLayer).ScheduleParallel(state.Dependency);

        //PhysicsDebug.DrawCollider(playerCollider, playerTransform, UnityEngine.Color.blue);
    }
}

struct DamagePlayerProcessor : IFindPairsProcessor
{
    public void Execute(in FindPairsResult result)
    {
        bool didHitWall = 
            Latios.Psyshock
                .Physics
                .DistanceBetween
                    ( result.bodyA.collider
                    , result.bodyA.transform
                    , result.bodyB.collider
                    , result.bodyB.transform
                    , 0f
                    , out var hitData
                    );

        //UnityEngine.Debug.LogFormat("Player might have hit a wall. Distance: {0}", hitData.distance);

        // UnityEngine.Debug.Log
        //     ( Latios.Psyshock
        //         .PhysicsDebug
        //         .LogDistanceBetween
        //             ( result.bodyA.collider
        //             , new Latios
        //                 .Psyshock
        //                 .PhysicsDebug
        //                 .TransformQvvs
        //                     ( result.bodyA.transform
        //                     )
        //             , result.bodyB.collider
        //             , new Latios
        //                 .Psyshock
        //                 .PhysicsDebug
        //                 .TransformQvvs
        //                     ( result.bodyB.transform
        //                     )
        //             , 0f
        //             )
        //     );

        if
            ( didHitWall
            )
        {
            UnityEngine.Debug.Log("Player hit a wall");
        }
    }


}

[WithAll(typeof(Character))]
partial struct PlayerColliderCastJob : IJobEntity
{
    public CollisionLayer TargetLayer;

    void Execute
        ( in Collider collider
        , in Rotation rotation
        , in Translation translation
        , in Velocity velocity
        )
    {
        RigidTransform transform =
            new RigidTransform
                ( rotation.Value
                , translation.Value - velocity.Value
                );

        //PhysicsDebug.DrawCollider(collider, transform, UnityEngine.Color.blue);

        // if
        //     ( Physics
        //         .ColliderCast
        //             ( collider
        //             , transform
        //             , translation.Value
        //             , TargetLayer
        //             , out ColliderCastResult result
        //             , out LayerBodyInfo layerBodyInfo
        //             )
        //     )
        // {
        //     UnityEngine.Debug.Log("Player hit a wall");
        // }
    }
}