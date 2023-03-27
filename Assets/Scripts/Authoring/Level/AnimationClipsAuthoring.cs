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
    public AnimationClip TurnSmallUp;
    public AnimationClip TurnSmallDown;
    public AnimationClip TurnSmallLeft;
    public AnimationClip TurnSmallRight;
    public AnimationClip TurnReverse;
    public AnimationClip Thrust;
    public AnimationClip WallKickShallow;
}

struct AnimationClipsSmartBakeItem : ISmartBakeItem<AnimationClipsAuthoring>
{
    SmartBlobberHandle<SkeletonClipSetBlob> blob;

    public bool Bake(AnimationClipsAuthoring authoring, IBaker baker)
    {
        baker.AddComponent<AnimationClips>();
        baker.AddComponent
            ( new CurrentAnimationClip
                { Index = AnimationClipIndex.LevelFlight
                , Start = 0f
                , Looping = true
                }
            );

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
        clips[(int) AnimationClipIndex.TurnSmallUp] =
            new SkeletonClipConfig
                { clip = authoring.TurnSmallUp
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };
        clips[(int) AnimationClipIndex.TurnSmallDown] =
            new SkeletonClipConfig
                { clip = authoring.TurnSmallDown
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };
        clips[(int) AnimationClipIndex.TurnSmallLeft] =
            new SkeletonClipConfig
                { clip = authoring.TurnSmallLeft
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };
        clips[(int) AnimationClipIndex.TurnSmallRight] =
            new SkeletonClipConfig
                { clip = authoring.TurnSmallRight
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };
        clips[(int) AnimationClipIndex.TurnReverse] = 
            new SkeletonClipConfig
                { clip = authoring.TurnReverse
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };
        clips[(int) AnimationClipIndex.Thrust] =
            new SkeletonClipConfig
                { clip = authoring.Thrust
                , settings = SkeletonClipCompressionSettings.kDefaultSettings
                };
        clips[(int) AnimationClipIndex.WallKickShallow] =
            new SkeletonClipConfig
                { clip = authoring.WallKickShallow
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