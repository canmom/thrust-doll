using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(ConfiguratorSystemGroup))]
[BurstCompile]
partial struct StateSpringSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<StateSpring>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config =
            SystemAPI
            .GetSingleton<StateSpringConfig>();

        var stateSpringJob = new StateSpringJob
            { DeltaTime = SystemAPI.Time.DeltaTime
            , Stiffness = config.Stiffness
            , Damping = config.Damping
            };

        stateSpringJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct StateSpringJob : IJobEntity
{
    public float DeltaTime;
    public float Stiffness;
    public float Damping;

    void Execute(ref StateSpring spring) {
        //semi-implicit Euler integration
        spring.Velocity += ((Stiffness * (spring.Target - spring.Displacement)) - (Damping * spring.Velocity)) * DeltaTime;
        spring.Displacement += spring.Velocity * DeltaTime;
    }
}