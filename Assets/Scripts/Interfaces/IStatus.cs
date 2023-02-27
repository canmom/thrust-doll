using Unity.Entities;

public interface IStatus : IComponentData
{
    public double TimeCreated { get; set; }
}