using Unity.Entities;
using Unity.Burst;

[BurstCompile]
partial struct TestSystem : ISystem
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
        var job = new TestJob();

        job.Schedule();
    }
}

public partial struct TestJob : IJobEntity
{
    void Execute(Entity e)
    {
        
    }
}