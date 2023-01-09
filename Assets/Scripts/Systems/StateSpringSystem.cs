using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct StateSpringSystem : ISystem
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
        var config = SystemAPI.GetSingleton<StateSpringConfig>();

        var stateSpringJob = new StateSpringJob
        {
            // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
            DeltaTime = SystemAPI.Time.DeltaTime,
            Stiffness = config.Stiffness,
            Damping = config.Damping
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

    void Execute([ChunkIndexInQuery] int chunkIndex, ref StateSpring spring) {
        //semi-implicit Euler integration
        spring.Velocity += ((Stiffness * (spring.Target - spring.Displacement)) - (Damping * spring.Velocity)) * DeltaTime;
        spring.Displacement += spring.Velocity * DeltaTime;
    }
}