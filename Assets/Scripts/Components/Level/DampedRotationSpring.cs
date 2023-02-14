using Unity.Entities;
using Unity.Mathematics;

partial struct DampedRotationSpring : IComponentData
{
    public float Stiffness;
    public float Damping;
}