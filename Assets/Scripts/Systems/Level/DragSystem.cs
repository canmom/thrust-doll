using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(ThrustAccelerationSystem))]
[UpdateBefore(typeof(VelocitySystem))]
partial struct DragSystem : ISystem
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

        new DampingJob
            { DeltaTime = deltaTime
            }
            .ScheduleParallel();
    }
}

partial struct DampingJob : IJobEntity
{
    public float DeltaTime;

    void Execute(ref Velocity velocity, in Drag drag)
    {
        velocity.Value -= drag.Coefficient * velocity.Value * math.length(velocity.Value);
    }
}