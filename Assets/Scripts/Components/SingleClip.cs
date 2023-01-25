using Unity.Entities;
using Latios.Kinemation;

public struct SingleClip : IComponentData
{
    public BlobAssetReference<SkeletonClipSetBlob> blob;
}