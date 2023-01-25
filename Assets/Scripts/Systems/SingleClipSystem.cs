using Latios;
using Latios.Kinemation;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial class SingleClipPlayerSystem : SubSystem
{
    protected override void OnUpdate()
    {
        float t = (float)SystemAPI.Time.ElapsedTime;

        Entities.ForEach((in DynamicBuffer<BoneReference> bones, in SingleClip singleClip) =>
        {
            ref var clip = ref singleClip.blob.Value.clips[0];

            var clipTime = clip.LoopToClipTime(t);

            for (int i = 1; i < bones.Length; i++)
            {
                var boneTransform = clip.SampleBone(i, clipTime);

                var trans = new Translation { Value = boneTransform.translation };
                var rot   = new Rotation { Value = boneTransform.rotation };
                // var scale = new NonUniformScale { Value = boneTransform.scale };

                Entity entity = bones[i].bone;

                SystemAPI.SetComponent(entity, trans);
                SystemAPI.SetComponent(entity, rot);
                // SystemAPI.SetComponent(entity, scale);
            }
        }).Schedule();
    }
}