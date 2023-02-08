using Unity.Entities;
using Unity.Burst;

[assembly: Unity.Jobs.RegisterGenericJobType(typeof(StatusCountdownJob<Thrust>))]
[BurstCompile]
partial struct StatusTickSystem : ISystem
{
    EntityTypeHandle entityHandle;

    ComponentTypeHandle<Thrust> thrustHandle;
    ComponentTypeHandle<ThrustCooldown> thrustCooldownHandle;

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
        entityHandle = SystemAPI.GetEntityTypeHandle();

        thrustHandle = SystemAPI.GetComponentTypeHandle<Thrust>(false);
        thrustCooldownHandle = SystemAPI.GetComponentTypeHandle<ThrustCooldown>(false);

        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityQuery thrustQuery = SystemAPI.QueryBuilder().WithAll<Thrust>().Build();
        EntityQuery thrustCooldownQuery = SystemAPI.QueryBuilder().WithAll<ThrustCooldown>().Build();

        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var thrustJob = new StatusCountdownJob<Thrust>
            {    DeltaTime = deltaTime
            ,          ECB = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            , StatusHandle = thrustHandle
            , EntityHandle = entityHandle
            };
        state.Dependency = thrustJob.Schedule(thrustQuery, state.Dependency);

        var thrustCooldownJob = new StatusCountdownJob<Thrust>
            {    DeltaTime = deltaTime
            ,          ECB = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            , StatusHandle = thrustHandle
            , EntityHandle = entityHandle
            };
        state.Dependency = thrustCooldownJob.Schedule(thrustCooldownQuery, state.Dependency);
        // var job2 = new StatusCountdownJob<ThrustCooldown> { DeltaTime = deltaTime };
        // job2.Schedule();
    }
}