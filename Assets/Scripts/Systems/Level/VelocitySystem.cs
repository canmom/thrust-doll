using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;

[UpdateInGroup(typeof(LevelSystemGroup))]
[BurstCompile]
partial struct VelocitySystem : ISystem
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

        new VelocityJob 
            { DeltaTime = deltaTime
            }
            .ScheduleParallel();
    }
}

[BurstCompile]
partial struct VelocityJob : IJobEntity
{
    public float DeltaTime;

    void Execute(in Velocity velocity, ref Translation position)
    {
        position.Value += velocity.Value * DeltaTime;
    }
}