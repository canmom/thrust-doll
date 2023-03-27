using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Latios.Kinemation;
using Unity.Collections;

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

        ComponentLookup<AnimationClipTimeOverride> overrideLookup =
            SystemAPI
                .GetComponentLookup<AnimationClipTimeOverride>();

        new AnimationClipPlayerJob
            { Time = time
            , OverrideLookup = overrideLookup
            }
            .ScheduleParallel();

        new AnimationTransitionJob
            { Time = time
            , ECB =
                ecbSystem
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .AsParallelWriter()
            , OverrideLookup = overrideLookup
            }
            .ScheduleParallel();
    }
}

[WithNone(typeof(TransientAnimationClip), typeof(AnimationTransition))]
partial struct AnimationClipPlayerJob : IJobEntity
{
    public float Time;
    [ReadOnly] public ComponentLookup<AnimationClipTimeOverride> OverrideLookup;

    void Execute
        ( ref DynamicBuffer<OptimizedBoneToRoot> btrBuffer
        , in OptimizedSkeletonHierarchyBlobReference hierarchyRef
        , in AnimationClips animationClips
        , in CurrentAnimationClip currentAnimation
        , Entity entity
        )
    {
        ref var clip =
            ref animationClips.blob.Value.clips[(int) currentAnimation.Index];

        float clipTime;
        
        if (OverrideLookup.TryGetComponent(entity, out AnimationClipTimeOverride timeOverride))
        {
            clipTime = timeOverride.ClipTime;
        } else {
            clipTime =
                currentAnimation.Looping
                    ? clip.LoopToClipTime(Time - currentAnimation.Start)
                    : Time - currentAnimation.Start;
        }

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
    [ReadOnly] public ComponentLookup<AnimationClipTimeOverride> OverrideLookup;

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

        float nextClipTime;
        
        if (OverrideLookup.TryGetComponent(entity, out AnimationClipTimeOverride timeOverride))
        {
            nextClipTime = timeOverride.ClipTime;
        } else {
            nextClipTime = Time - transition.Start;
        }

        float nextWeight = math.smoothstep(0f, transition.Duration, nextClipTime);

        currentClip.SamplePose
            ( ref blender
            , 1 - nextWeight
            , currentClipTime
            );

        nextClip.SamplePose
            ( ref blender
            , nextWeight
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