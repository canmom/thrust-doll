using Unity.Entities;

partial struct CollidesWithSDF : IComponentData
{
    public float Radius;
    public float InnerRadius;
}