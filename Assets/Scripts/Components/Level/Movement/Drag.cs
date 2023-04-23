using Unity.Entities;

partial struct Drag : IComponentData
{
    public float Coefficient;
}

partial struct AngularDamping : IComponentData
{
    public float Coefficient;
}