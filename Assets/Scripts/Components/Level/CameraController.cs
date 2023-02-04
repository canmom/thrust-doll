using Unity.Entities;
using Unity.Mathematics;

struct CameraController : IComponentData
{
    public quaternion Orientation;
    public float Distance;
}