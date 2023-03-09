using Unity.Entities;

partial struct TransientAnimationClip : IComponentData
{
    public AnimationClipIndex Index;

    public float TimeCreated;

    public float StartupEnd;
    public float RecoveryStart;
    public float AnimationEnd;
}