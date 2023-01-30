using Latios;
using Latios.Kinemation;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial class SingleClipSystem : SubSystem
{
    protected override void OnCreate()
    {
        RequireForUpdate<SingleClip>();
    }

    protected override void OnUpdate()
    {
        float t = (float)SystemAPI.Time.ElapsedTime;

        Entities.ForEach((ref Translation trans, ref Rotation rot, ref NonUniformScale scale, in BoneOwningSkeletonReference skeletonRef, in BoneIndex boneIndex) =>
        {
            if (boneIndex.index == 0)
                return;
            var singleClip = SystemAPI.GetComponent<SingleClip>(skeletonRef.skeletonRoot);

            ref var clip     = ref singleClip.blob.Value.clips[0];
            var     clipTime = clip.LoopToClipTime(t);

            var boneTransform = clip.SampleBone(boneIndex.index, clipTime);

            trans.Value = boneTransform.translation;
            rot.Value   = boneTransform.rotation;
            scale.Value = boneTransform.scale;
        }).ScheduleParallel();
    }
}