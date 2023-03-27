using Unity.Entities;
using Unity.Mathematics;

partial struct SDFCollision : IComponentData
{
    public float Distance;
    public float3 Normal;
}