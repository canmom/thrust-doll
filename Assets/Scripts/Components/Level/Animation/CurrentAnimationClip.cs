using Unity.Entities;

partial struct CurrentAnimationClip : IComponentData
{
    public AnimationClipIndex Index;

    public float Start;

    public bool Looping;
}