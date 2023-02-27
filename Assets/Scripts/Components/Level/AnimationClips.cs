using Unity.Entities;
using Latios.Kinemation;

public struct AnimationClips : IComponentData
{
    public BlobAssetReference<SkeletonClipSetBlob> blob;
}