using Unity.Entities;

partial struct DynamicAnimationClip : IComponentData
{
    public AnimationClipIndex Index;

    public float SampleTime;

    public float Weight;
}