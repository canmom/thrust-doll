using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateBefore(typeof(AngularVelocitySystem))]
partial struct RotationSpringSystem : ISystem
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

        new RotationSpringJob
            { DeltaTime = deltaTime
            }
            .ScheduleParallel();
    }
}

[BurstCompile]
partial struct RotationSpringJob : IJobEntity
{
    public float DeltaTime;

    void Execute
        ( ref AngularVelocity angularVelocity
        , in Rotation rotation
        , in DampedRotationSpring spring
        , in RotationTarget target
        )
    {
        float3 currentOrientation = math.mul(rotation.Value, new float3 (0, 0, 1));
        float3 torque = spring.Stiffness * math.cross(currentOrientation, target.Target);
        angularVelocity.Value += (torque - spring.Damping * angularVelocity.Value) * DeltaTime;
    }
}