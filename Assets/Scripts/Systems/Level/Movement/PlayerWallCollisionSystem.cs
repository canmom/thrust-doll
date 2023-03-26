using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(SDFCollisionSystem))]
partial struct PlayerWallCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // foreach
        //     ( var (velocity, collision) in
        //         SystemAPI
        //             .Query<RefRW<Velocity>,RefRO<SDFCollision>>()
        //     )
        // {
        //     velocity.ValueRW.Value = new float3(0f);
        // }
    }
}