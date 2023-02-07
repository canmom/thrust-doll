using Unity.Entities;
using Unity.Burst;

[assembly: Unity.Jobs.RegisterGenericJobType(typeof(StatusCountdownJob<Thrust>))]
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
        float deltaTime = SystemAPI.Time.DeltaTime;

        var job = new StatusCountdownJob<Thrust> { DeltaTime = deltaTime };
        job.Schedule();
        // var job2 = new StatusCountdownJob<ThrustCooldown> { DeltaTime = deltaTime };
        // job2.Schedule();
    }
}