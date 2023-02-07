using Unity.Entities;
using Unity.Mathematics;

public struct Intent : IComponentData
{
    public float2 Rotate;
    public bool Thrust;
}