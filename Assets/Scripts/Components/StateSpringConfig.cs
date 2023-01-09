using Unity.Entities;

struct StateSpringConfig : IComponentData
{
    public float Stiffness;
    public float Damping;
}