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
public struct ThrustActive : IStatus, IComponentData
{
    public double TimeCreated { get; set; }
}

[BurstCompile]
public struct ThrustWindup : IStatus, IComponentData
{
    public double TimeCreated { get; set; }

    public float PreviousDrag;
}