using System;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
partial struct UIToggleJob : IJobEntity
{
    public float DeltaTime;

    void Execute([ChunkIndexInQuery] int chunkIndex, ref UIToggleAspect uiToggle)
    {
        uiToggle.Opacity = uiToggle.On ? 0.6f : 0f;
        uiToggle.Scale = uiToggle.Hover ? 0.04f : 0.01f;
        uiToggle.Thickness = uiToggle.Hover ? 0.125f : (uiToggle.On ? 0f : 0.5f);
    }
}

[BurstCompile]
partial struct UIToggleSystem : ISystem
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
        var uiToggleJob = new UIToggleJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        uiToggleJob.ScheduleParallel();
    }
}
