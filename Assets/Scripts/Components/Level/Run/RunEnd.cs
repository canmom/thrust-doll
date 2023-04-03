using Unity.Entities;

partial struct RunEnd : IComponentData
{
    public double Time;
    public double Duration;
}