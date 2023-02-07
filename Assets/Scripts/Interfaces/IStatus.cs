using Unity.Entities;

public interface IStatus : IComponentData
{
    public float TimeRemaining { get; set; }
}