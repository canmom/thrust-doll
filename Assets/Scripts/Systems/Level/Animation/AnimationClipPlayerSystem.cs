using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Latios.Kinemation;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
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

        ComponentLookup<AnimationClipTimeOverride> overrideLookup =
            SystemAPI
                .GetComponentLookup<AnimationClipTimeOverride>();

        new AnimationClipPlayerJob
            { Time = time
            }
            .ScheduleParallel();

        new AnimationTransitionJob
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
[BurstCompile]
partial struct AnimationClipPlayerJob : IJobEntity
{
    public float Time;

    [BurstCompile]
    void Execute
        ( ref DynamicBuffer<OptimizedBoneToRoot> btrBuffer
        , in OptimizedSkeletonHierarchyBlobReference hierarchyRef
        , in AnimationClips animationClips
        , in CurrentAnimationClip currentAnimation
        , in DynamicAnimationClip dynamicAnimation
        , Entity entity
        )
    {
        ref var clip =
            ref animationClips.blob.Value.clips[(int) currentAnimation.Index];

        ref var dynamicClip =
            ref animationClips.blob.Value.clips[(int) dynamicAnimation.Index];

        BufferPoseBlender blender = new BufferPoseBlender(btrBuffer);

        float clipTime =
            currentAnimation.Looping
                ? clip.LoopToClipTime(Time - currentAnimation.Start)
                : Time - currentAnimation.Start;

        clip.SamplePose
            ( ref blender
            , 1 - dynamicAnimation.Weight
            , clipTime
            );

        dynamicClip.SamplePose
            ( ref blender
            , dynamicAnimation.Weight
            , dynamicAnimation.SampleTime
            );

        blender.NormalizeRotations();

        blender.ApplyBoneHierarchyAndFinish(hierarchyRef.blob);
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
        , in DynamicAnimationClip dynamicAnimation
        , Entity entity
        , [ChunkIndexInQuery] int chunkIndex
        )
    {
        BufferPoseBlender blender = new BufferPoseBlender(btrBuffer);

        ref var currentClip =
            ref animationClips.blob.Value.clips[(int) current.Index];
        ref var nextClip =
            ref animationClips.blob.Value.clips[(int) transition.NextIndex];

        ref var dynamicClip =
            ref animationClips.blob.Value.clips[(int) dynamicAnimation.Index];

        float currentClipTime =
            current.Looping
                ? currentClip.LoopToClipTime(Time - current.Start)
                : Time - current.Start;

        float nextClipTime = Time - transition.Start;

        float nextWeight = math.smoothstep(0f, transition.Duration, nextClipTime);

        currentClip.SamplePose
            ( ref blender
            , (1 - nextWeight) * (1 - dynamicAnimation.Weight)
            , currentClipTime
            );

        nextClip.SamplePose
            ( ref blender
            , nextWeight * (1 - dynamicAnimation.Weight)
            , nextClipTime
            );

        dynamicClip.SamplePose
            ( ref blender
            , dynamicAnimation.Weight
            , dynamicAnimation.SampleTime
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

