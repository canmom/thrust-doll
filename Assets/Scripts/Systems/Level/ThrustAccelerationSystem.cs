using Unity.Entities;
using Unity.Burst;

[UpdateBefore(typeof(VelocitySystem))]
[BurstCompile]
partial struct ThrustAccelerationSystem : ISystem
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
        float deltaTime =
            SystemAPI
                .Time
                .DeltaTime;

        new ThrustAccelerationJob { DeltaTime = deltaTime }
            .Schedule();
    }
}

[BurstCompile]
public partial struct ThrustAccelerationJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(in Thrust thrust, ref Velocity velocity)
    {
        velocity.Value += thrust.Acceleration * DeltaTime;
    }
}

