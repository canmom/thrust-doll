using Unity.Entities;
using Unity.Mathematics;

partial struct WallKick : IComponentData
{
    public float3 IncidentVelocity;
    public float3 ReflectionVelocity;
    public float3 Normal;
    public double TimeCreated;
}

partial struct ClearingWall : IComponentData
{

}

partial struct FaceWall : IComponentData
{
    public quaternion InitialRotation;
}