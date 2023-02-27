using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[DisallowMultipleComponent]
public class AnimationClipsAuthoring : MonoBehaviour
{
    public AnimationClip FlightClip;
    public AnimationClip TurnUpSmallClip;
}

struct AnimationClipsSmartBakeItem : ISmartBakeItem<AnimationClipsAuthoring>
{
    SmartBlobberHandle<SkeletonClipSetBlob> blob;

    public bool Bake(AnimationClipsAuthoring authoring, IBaker baker)
    {
        baker.AddComponent<AnimationClips>();
        var clips = new NativeArray<SkeletonClipConfig>(2, Allocator.Temp);
        clips[0]  = new SkeletonClipConfig { clip = authoring.FlightClip, settings = SkeletonClipCompressionSettings.kDefaultSettings };
        clips[1]  = new SkeletonClipConfig { clip = authoring.TurnUpSmallClip, settings = SkeletonClipCompressionSettings.kDefaultSettings };
        blob      = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
        return true;
    }

    public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
    {
        entityManager.SetComponentData(entity, new AnimationClips { blob = blob.Resolve(entityManager) });
    }
}

class AnimationClipsBaker : SmartBaker<AnimationClipsAuthoring, AnimationClipsSmartBakeItem>
{
}