using Unity.Entities;
using Unity.Mathematics;

partial struct SDFCollision : IComponentData
{
    public float3 Distance;
    public float3 Normal;
}