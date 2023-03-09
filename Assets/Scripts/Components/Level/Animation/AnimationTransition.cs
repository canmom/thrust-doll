using Unity.Entities;

partial struct AnimationTransition : IComponentData
{
    public AnimationClipIndex NextAnimation;

    public float Duration;
    public float TimeCreated;
}