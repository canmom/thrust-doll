using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct Thrust : IStatus, IComponentData
{
    public float TimeRemaining { get; set; }

    public float3 Acceleration;
}