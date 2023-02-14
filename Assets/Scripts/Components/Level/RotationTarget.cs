using Unity.Entities;
using Unity.Mathematics;

partial struct RotationTarget : IComponentData
{
    public float3 Target;
}