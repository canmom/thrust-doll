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

[WithNone(typeof(TransientAnimationClip), typeof(AnimationTransition))]
partial struct AnimationClipPlayerJob : IJobEntity
{
    public float Time;

    void Execute
        ( ref DynamicBuffer<OptimizedBoneToRoot> btrBuffer
        , in OptimizedSkeletonHierarchyBlobReference hierarchyRef
        , in AnimationClips animationClips
        , in CurrentAnimationClip currentAnimation
        )
    {
        ref var clip =
            ref animationClips.blob.Value.clips[(int) currentAnimation.Index];
        float clipTime =
            currentAnimation.Looping
                ? clip.LoopToClipTime(Time - currentAnimation.Start)
                : Time - currentAnimation.Start;

        clip.SamplePose
            ( btrBuffer
            , hierarchyRef.blob
            , clipTime
            );
    }
}

partial struct AnimationTransitionJob : IJobEntity
{
    public float Time;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute
        ( ref DynamicBuffer<OptimizedBoneToRoot> btrBuffer
        , in OptimizedSkeletonHierarchyBlobReference hierarchyRef
        , in AnimationClips animationClips
        , ref CurrentAnimationClip current
        , in AnimationTransition transition
        , Entity entity
        , [ChunkIndexInQuery] int chunkIndex
        )
    {
        BufferPoseBlender blender = new BufferPoseBlender(btrBuffer);

        ref var currentClip =
            ref animationClips.blob.Value.clips[(int) current.Index];
        ref var nextClip =
            ref animationClips.blob.Value.clips[(int) transition.NextIndex];

        float currentClipTime =
            current.Looping
                ? currentClip.LoopToClipTime(Time - current.Start)
                : Time - current.Start;
        float nextClipTime = Time - transition.Start;

        float nextWeight = math.smoothstep(0f, transition.Duration, nextClipTime);

        currentClip.SamplePose
            ( ref blender
            , 1 - nextWeight
            , currentClipTime
            );

        nextClip.SamplePose
            ( ref blender
            , 1 - nextWeight
            , nextClipTime
            );

        blender.NormalizeRotations();

        blender.ApplyBoneHierarchyAndFinish(hierarchyRef.blob);

        if (nextClipTime > transition.Duration)
        {
            current.Index = transition.NextIndex;
            current.Start = transition.Start;
            current.Looping = transition.Looping;

            ECB.RemoveComponent<AnimationTransition>(chunkIndex, entity);
        }
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