using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
partial struct StatusTickSystem : ISystem
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
        EntityTypeHandle entityHandle =
            SystemAPI.GetEntityTypeHandle();

        ComponentTypeHandle<Thrust> thrustHandle =
            SystemAPI
                .GetComponentTypeHandle<Thrust>(false);
        ComponentTypeHandle<ThrustCooldown> thrustCooldownHandle =
            SystemAPI
                .GetComponentTypeHandle<ThrustCooldown>(false);

        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityQuery thrustQuery =
            SystemAPI
                .QueryBuilder()
                .WithAll<Thrust>()
                .Build();
        EntityQuery thrustCooldownQuery =
            SystemAPI
                .QueryBuilder()
                .WithAll<ThrustCooldown>()
                .Build();

        var ecbSystem =
            SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var thrustJob = new StatusCountdownJob<Thrust>
            {    DeltaTime = deltaTime
            ,          ECB = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            , StatusHandle = thrustHandle
            , EntityHandle = entityHandle
            };
        state.Dependency =
            thrustJob
                .Schedule
                    ( thrustQuery
                    , state.Dependency
                    );

        var thrustCooldownJob = new StatusCountdownJob<ThrustCooldown>
            {    DeltaTime = deltaTime
            ,          ECB = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            , StatusHandle = thrustCooldownHandle
            , EntityHandle = entityHandle
            };
        state.Dependency =
            thrustCooldownJob
                .Schedule
                    ( thrustCooldownQuery
                    , state.Dependency
                    );
    }

    // [BurstCompile]
    // void scheduleJob<T>(ref SystemState state)
    //     where T: unmanaged, IComponentData, IStatus
    // {
    //     ComponentTypeHandle<T> statusHandle = SystemAPI.GetComponentTypeHandle<T>(false);
    //     EntityTypeHandle entityHandle = SystemAPI.GetEntityTypeHandle();

    //     float deltaTime = SystemAPI.Time.DeltaTime;

    //     EntityQuery statusQuery = SystemAPI.QueryBuilder().WithAll<T>().Build();

    //     var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

    //     var job = new StatusCountdownJob<T>
    //         {    DeltaTime = deltaTime
    //         ,          ECB = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
    //         , StatusHandle = statusHandle
    //         , EntityHandle = entityHandle
    //         };
    //     state.Dependency = job.ScheduleParallel(statusQuery, state.Dependency);
    // }
}