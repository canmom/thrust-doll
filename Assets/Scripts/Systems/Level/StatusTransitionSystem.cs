using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

[assembly: RegisterGenericJobType(typeof(StatusEndJob<ThrustCooldown>))]
[assembly: RegisterGenericJobType(typeof(StatusEndJob<RotateTo>))]

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
partial struct StatusTransitionSystem : ISystem, ISystemStartStop
{
    StatusJobFields<RotateTo> rotateToFields;
    StatusJobFields<ThrustCooldown> thrustCooldownFields;

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        var queryBuilder = new EntityQueryBuilder(Allocator.Temp);

        Level levelSettings = SystemAPI.GetSingleton<Level>();

        thrustCooldownFields = getStatusFields<ThrustCooldown>(ref state, queryBuilder, levelSettings.ThrustCooldown);
        rotateToFields = getStatusFields<RotateTo>(ref state, queryBuilder, levelSettings.ThrustWindup);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        scheduleEndJob<ThrustCooldown>(ref state, thrustCooldownFields);
        scheduleEndJob<RotateTo>(ref state, rotateToFields);
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }

    [BurstCompile]
    StatusJobFields<T> getStatusFields<T>(ref SystemState state, EntityQueryBuilder queryBuilder, double duration)
        where T: unmanaged, IComponentData, IStatus
    {
        EntityQuery query =
            queryBuilder
                .WithAll<T>()
                .Build(ref state);

        queryBuilder.Reset();

        return new StatusJobFields<T>
            { Handle = state.GetComponentTypeHandle<T>(true)
            , Query = query
            , Duration = duration
            };
    }

    [BurstCompile]
    void scheduleEndJob<T>(ref SystemState state, StatusJobFields<T> fields)
        where T: unmanaged, IComponentData, IStatus
    {
        fields.Handle.Update(ref state);

        EntityTypeHandle entityHandle =
            SystemAPI
                .GetEntityTypeHandle();

        double elapsedTime =
            SystemAPI
                .Time
                .ElapsedTime;

        var ecbSystem =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var job = new StatusEndJob<T>
            { Time = elapsedTime
            , Duration = fields.Duration
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .AsParallelWriter()
            , StatusHandle = fields.Handle
            , EntityHandle = entityHandle
            };
        state.Dependency =
            job
                .ScheduleParallel
                    ( fields.Query
                    , state.Dependency
                    );
    }

    void scheduleChainJob<T,U>(ref SystemState state, StatusJobFields<T> fields)
        where T: unmanaged, IComponentData, IStatus
        where U: unmanaged, IComponentData, IStatus
    {
        fields.Handle.Update(ref state);

        EntityTypeHandle entityHandle =
            SystemAPI
                .GetEntityTypeHandle();

        double elapsedTime =
            SystemAPI
                .Time
                .ElapsedTime;

        var ecbSystem =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var job = new StatusChainJob<T,U>
            { Time = elapsedTime
            , Duration = fields.Duration
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .AsParallelWriter()
            , StatusHandle = fields.Handle
            , EntityHandle = entityHandle
            };
        state.Dependency =
            job
                .ScheduleParallel
                    ( fields.Query
                    , state.Dependency
                    );
    }
}

struct StatusJobFields<T>
    where T: unmanaged, IComponentData, IStatus
{
    public ComponentTypeHandle<T> Handle;
    public EntityQuery Query;
    public double Duration;
}