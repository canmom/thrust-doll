using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct Thrust : IStatus, IComponentData
{
    public double TimeCreated { get; set; }

    public float3 Acceleration;
}

[BurstCompile]
public struct ThrustRotation : IStatus, IComponentData
{
    public double TimeCreated { get; set; }

    public quaternion InitialRotation;
    public quaternion TargetRotation;

    public bool BeforeActive;
}

[BurstCompile]
public struct ThrustFlip : IStatus, IComponentData
{
    public double TimeCreated { get; set; }

    public quaternion InitialRotation;
    public quaternion BackRotation;
    public quaternion TargetRotation;

    public float PreviousDrag;
}

[BurstCompile]
public struct ThrustActive : IStatus, IComponentData
{
    public double TimeCreated { get; set; }
}