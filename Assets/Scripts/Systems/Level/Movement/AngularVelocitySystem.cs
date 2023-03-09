using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
partial struct AngularVelocitySystem : ISystem
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
        
        new AngularVelocityJob
            { DeltaTime = deltaTime
            }
            .ScheduleParallel();
    }
}

partial struct AngularVelocityJob : IJobEntity
{
    public float DeltaTime;

    void Execute(ref Rotation rotation, in AngularVelocity angularVelocity)
    {
        float angle =
            math.length(angularVelocity.Value) * DeltaTime;

        quaternion deltaRotation =
            quaternion
                .AxisAngle
                    ( angularVelocity.Value
                    , angle
                    );

        rotation.Value =
            math.normalize
                ( math.mul
                    ( deltaRotation
                    , rotation.Value
                    )
                );
    }
}