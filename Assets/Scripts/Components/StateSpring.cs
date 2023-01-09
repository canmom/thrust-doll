using Unity.Entities;

struct StateSpring : IComponentData
{
    public float Displacement;
    public float Velocity;
    public float Target;
}