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

        new DragJob
            { DeltaTime = deltaTime
            }
            .ScheduleParallel();

        new AngularDampingJob
            { DeltaTime = deltaTime
            }
            .ScheduleParallel();
    }
}

[BurstCompile]
partial struct DragJob : IJobEntity
{
    public float DeltaTime;

    void Execute(ref Velocity velocity, in Drag drag)
    {
        velocity.Value -= drag.Coefficient * velocity.Value * math.length(velocity.Value) * DeltaTime;
    }
}

[BurstCompile]
partial struct AngularDampingJob : IJobEntity
{
    public float DeltaTime;

    void Execute(ref AngularVelocity angularVelocity, in AngularDamping angularDamping)
    {
        angularVelocity.Value *= (1 - angularDamping.Coefficient * DeltaTime);
    }
}