using Unity.Entities;

partial struct AnimationTransition : IComponentData
{
    public AnimationClipIndex NextIndex;

    public float Start;

    public bool Looping;

    public float Duration;
}