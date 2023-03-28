using Unity.Entities;
using Unity.Burst;
using Latios;
using Latios.Systems;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(SDFCollisionSystem))]
partial struct ProjectileWallCollisionSystem : ISystem
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
        SyncPointPlaybackSystem syncPointSystem =
            _latiosWorld.syncPoint;

        DestroyCommandBuffer.ParallelWriter dcb =
            syncPointSystem
                .CreateDestroyCommandBuffer()
                .AsParallelWriter();

        new ProjectileWallCollisionJob
            { DCB = dcb
            }
            .ScheduleParallel();
    }
}

[WithAll(typeof(Projectile), typeof(SDFCollision))]
partial struct ProjectileWallCollisionJob : IJobEntity
{
    public DestroyCommandBuffer.ParallelWriter DCB;

    void Execute
        ( Entity entity
        , [ChunkIndexInQuery] int chunkIndex
        )
    {
        DCB.Add(entity, chunkIndex);
    }
}