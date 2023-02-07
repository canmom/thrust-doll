using Unity.Entities;
using Unity.Mathematics;

public struct CameraController : IComponentData
{
    public float Pitch;
    public float Yaw;
    public float Distance;
}