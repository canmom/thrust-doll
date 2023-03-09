using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Latios;
using Latios.Psyshock;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(PlayerCollisionLayerSystem))]
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
                    ( projectileCollisionLayer
                    , playerCollisionLayer
                    , processor
                    )
                    .ScheduleParallel(state.Dependency);
    }
}

struct DamagePlayerProcessor : IFindPairsProcessor
{
    public void Execute(in FindPairsResult result)
    {
        if
            ( Latios.Psyshock
                .Physics
                .DistanceBetween
                    ( result.bodyA.collider
                    , result.bodyA.transform
                    , result.bodyB.collider
                    , result.bodyB.transform
                    , 0f
                    , out var hitData
                    )
            )
        {
            Debug.Log("Player was hit by a bullet");
        }
    }


}