using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
[UpdateAfter(typeof(MouseHandlingSystem))]
partial struct UIToggleJob : IJobEntity
{
    public float DeltaTime;
    [ReadOnly] public ComponentLookup<Hovering> HoveringLookup;
    [ReadOnly] public ComponentLookup<On> OnLookup;

    void Execute([ChunkIndexInQuery] int chunkIndex, ref UIToggleAspect uiToggle, in Parent parent)
    {
        bool hovering = HoveringLookup.IsComponentEnabled(parent.Value);
        bool isOn = OnLookup.IsComponentEnabled(parent.Value);

        //set the target for animation
        uiToggle.StateTarget = hovering ? 1f : 0f;

        //displacement for use in LERP; 0 to 1 with overshoots possible
        float disp = uiToggle.StateDisplacement;

        uiToggle.Opacity = isOn ? 0.6f : 0f;
        uiToggle.Scale = 0.01f + 0.02f * disp;
        //uiToggle.Thickness = hovering ? 0.2f : (isOn ? 0f : 0.6f);
        uiToggle.Thickness = isOn ? 0f + 0.2f * disp : 0.6f - 0.4f * disp;
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
            DeltaTime = SystemAPI.Time.DeltaTime,
            HoveringLookup = SystemAPI.GetComponentLookup<Hovering>(true),
            OnLookup = SystemAPI.GetComponentLookup<On>(true)
        };
        uiToggleJob.ScheduleParallel();
    }
}
