using Unity.Entities;
using Unity.Burst;
using Latios.Kinemation;

[BurstCompile]
partial struct AnimationClipPlayerSystem : ISystem
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
        float time = (float) SystemAPI.Time.ElapsedTime;

        new AnimationClipPlayerJob
            { Time = time
            }
            .ScheduleParallel();
    }
}

partial struct AnimationClipPlayerJob : IJobEntity
{
    public float Time;

    void Execute(ref DynamicBuffer<OptimizedBoneToRoot> btrBuffer, in OptimizedSkeletonHierarchyBlobReference hierarchyRef, in AnimationClips animationClips)
    {
        ref var clip = ref animationClips.blob.Value.clips[0];
        var clipTime = clip.LoopToClipTime(Time);

        clip.SamplePose
            ( btrBuffer
            , hierarchyRef.blob
            , clipTime
            );
    }
}