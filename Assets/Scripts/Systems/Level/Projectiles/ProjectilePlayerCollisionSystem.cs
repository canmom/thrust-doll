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
partial struct ProjectilePlayerCollisionSystem : ISystem
{
    LatiosWorldUnmanaged _latiosWorld;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _latiosWorld = state.GetLatiosWorldUnmanaged();

        state.RequireForUpdate<Run>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BlackboardEntity blackboard =
            _latiosWorld
                .sceneBlackboardEntity;

        var projectileCollisionLayer =
            blackboard
                .GetCollectionComponent<ProjectileCollisionLayer>(true)
                .Layer;
        
        EntityCommandBuffer ecb =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

        new PlayerColliderCastJob
            { TargetLayer = projectileCollisionLayer
            , Blackboard = blackboard
            , Time = SystemAPI.Time.ElapsedTime
            , DeltaTime = SystemAPI.Time.DeltaTime
            , RunStartTime = blackboard.GetComponentData<Run>().Start
            , ECB = ecb
            , VelocityLookup = SystemAPI.GetComponentLookup<Velocity>()
            , BulletMass = blackboard.GetComponentData<Level>().BulletMass
            }
            .Schedule();

        // var playerCollisionLayer =
        //     _latiosWorld
        //         .sceneBlackboardEntity
        //         .GetCollectionComponent<PlayerCollisionLayer>(true)
        //         .Layer;

        // var processor = new DamagePlayerProcessor
        // {

        // };

        // state.Dependency =
        //     Latios.Psyshock.Physics
        //         .FindPairs
        //             ( projectileCollisionLayer
        //             , playerCollisionLayer
        //             , processor
        //             )
        //             .ScheduleParallel(state.Dependency);

        //state.Dependency = PhysicsDebug.DrawLayer(wallCollisionLayer).ScheduleParallel(state.Dependency);

        //PhysicsDebug.DrawCollider(playerCollider, playerTransform, UnityEngine.Color.blue);
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
            UnityEngine.Debug.Log("Player was hit by a bullet.");
        }
    }


}

[WithAll(typeof(Character))]
partial struct PlayerColliderCastJob : IJobEntity
{
    public CollisionLayer TargetLayer;

    public Entity Blackboard;

    public double Time;
    public float DeltaTime;

    public double RunStartTime;

    public EntityCommandBuffer ECB;

    public ComponentLookup<Velocity> VelocityLookup;

    public float BulletMass;

    void Execute
        ( in Collider collider
        , in Rotation rotation
        , in Translation translation
        , ref AngularVelocity angularVelocity
        , Entity playerEntity
        )
    {
        float3 velocity = VelocityLookup[playerEntity].Value;

        RigidTransform transform =
            new RigidTransform
                ( rotation.Value
                , translation.Value - velocity * DeltaTime
                );

        //PhysicsDebug.DrawCollider(collider, transform, UnityEngine.Color.blue);

        if
            ( Physics
                .ColliderCast
                    ( collider
                    , transform
                    , translation.Value
                    , TargetLayer
                    , out ColliderCastResult result
                    , out LayerBodyInfo layerBodyInfo
                    )
            )
        {
            float3 hitpoint = result.hitpointOnCaster;
            float distance = result.distance;

            float3 dollPositionAtHit =
                translation.Value
                - (1 - distance / math.length(velocity)) 
                * velocity;

            float3 momentArm =
                hitpoint - dollPositionAtHit;

            float3 bulletVelocity =
                VelocityLookup[layerBodyInfo.entity].Value;

            float3 bulletVelocityInDollFrame =
                bulletVelocity - velocity;

            float3 torqueImpulse =
                BulletMass
                * math.cross
                    ( momentArm
                    , bulletVelocityInDollFrame
                    );

            ECB.AddComponent
                ( Blackboard
                , new RunEnd
                    { Time = Time
                    , Duration = Time - RunStartTime
                    }
                );
        }
    }
}