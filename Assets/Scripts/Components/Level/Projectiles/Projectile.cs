using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

partial struct Projectile : IComponentData
{
    
}

[MaterialProperty("_InstanceOffset")]
partial struct ProjectileShaderOffset : IComponentData
{
    public float2 Offset;
}