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
    public AnimationClip Flight;
    public AnimationClip TurnUpSmall;
    public AnimationClip Thrust;
}

struct AnimationClipsSmartBakeItem : ISmartBakeItem<AnimationClipsAuthoring>
{
    SmartBlobberHandle<SkeletonClipSetBlob> blob;

    public bool Bake(AnimationClipsAuthoring authoring, IBaker baker)
    {
        baker.AddComponent<AnimationClips>();

        var clips =
            new NativeArray<SkeletonClipConfig>
                ( System.Enum
                    .GetNames(typeof(AnimationClipIndex))
                    .Length
                , Allocator.Temp
                );

        clips[(int) AnimationClipIndex.LevelFlight] =
            new SkeletonClipConfig
                { clip = authoring.Flight
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };
        clips[(int) AnimationClipIndex.TurnUpSmall] =
            new SkeletonClipConfig
                { clip = authoring.TurnUpSmall
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };
        clips[(int) AnimationClipIndex.Thrust] =
            new SkeletonClipConfig
                { clip = authoring.Thrust
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };

        blob = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
        return true;
    }

    public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
    {
        entityManager
            .SetComponentData
                ( entity
                , new AnimationClips
                    { blob = blob.Resolve(entityManager)
                    }
                );
    }
}

class AnimationClipsBaker : SmartBaker<AnimationClipsAuthoring, AnimationClipsSmartBakeItem>
{
}