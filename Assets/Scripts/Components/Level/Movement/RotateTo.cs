using Unity.Entities;
using Unity.Mathematics;

public struct RotateTo : IStatus, IComponentData
{
    public double TimeCreated { get; set; }

    public quaternion InitialRotation;
    public quaternion TargetRotation;

    public float Duration;
}

public struct Flip : IStatus, IComponentData
{
    public double TimeCreated { get; set; }

    public quaternion InitialRotation;
    public quaternion BackRotation;
    public quaternion TargetRotation;
}