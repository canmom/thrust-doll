using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
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

        var ecbSystem =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new AnimationClipPlayerJob
            { Time = time
            }
            .ScheduleParallel();

        new TransientClipBlenderJob
            { Time = time
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .AsParallelWriter()
            }
            .ScheduleParallel();
    }
}

[WithNone(typeof(TransientAnimationClip))]
partial struct AnimationClipPlayerJob : IJobEntity
{
    public float Time;

    void Execute
        ( ref DynamicBuffer<OptimizedBoneToRoot> btrBuffer
        , in OptimizedSkeletonHierarchyBlobReference hierarchyRef
        , in AnimationClips animationClips
        )
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

partial struct TransientClipBlenderJob : IJobEntity
{
    public float Time;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute
        ( ref DynamicBuffer<OptimizedBoneToRoot> btrBuffer
        , in OptimizedSkeletonHierarchyBlobReference hierarchyRef
        , in AnimationClips animationClips
        , in TransientAnimationClip tc
        , Entity entity
        , [ChunkIndexInQuery] int chunkIndex
        )
    {
        BufferPoseBlender blender = new BufferPoseBlender(btrBuffer);

        ref var baseClip =
            ref animationClips.blob.Value.clips[0];
        ref var transientClip =
            ref animationClips.blob.Value.clips[(int) tc.Index];

        float baseClipTime = baseClip.LoopToClipTime(Time);
        float transientClipTime = Time - tc.TimeCreated;

        float transientWeight = 
            math.smoothstep(0f, tc.StartupEnd, transientClipTime)
            * ( 1 - math.smoothstep(tc.RecoveryStart, tc.AnimationEnd, transientClipTime));

        baseClip.SamplePose
            ( ref blender
            , 1 - transientWeight
            , baseClipTime
            );

        transientClip.SamplePose
            ( ref blender
            , transientWeight
            , transientClipTime
            );

        blender.NormalizeRotations();

        blender.ApplyBoneHierarchyAndFinish(hierarchyRef.blob);

        if (transientClipTime > tc.AnimationEnd)
        {
            ECB.RemoveComponent<TransientAnimationClip>(chunkIndex, entity);
        }
    }
}