using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[DisallowMultipleComponent]
public class SingleClipAuthoring : MonoBehaviour
{
    public AnimationClip clip;
}

struct SingleClipSmartBakeItem : ISmartBakeItem<SingleClipAuthoring>
{
    SmartBlobberHandle<SkeletonClipSetBlob> blob;

    public bool Bake(SingleClipAuthoring authoring, IBaker baker)
    {
        baker.AddComponent<SingleClip>();
        var clips = new NativeArray<SkeletonClipConfig>(1, Allocator.Temp);
        clips[0]  = new SkeletonClipConfig { clip = authoring.clip, settings = SkeletonClipCompressionSettings.kDefaultSettings };
        blob      = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
        return true;
    }

    public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
    {
        entityManager.SetComponentData(entity, new SingleClip { blob = blob.Resolve(entityManager) });
    }
}

class SingleClipBaker : SmartBaker<SingleClipAuthoring, SingleClipSmartBakeItem>
{
}