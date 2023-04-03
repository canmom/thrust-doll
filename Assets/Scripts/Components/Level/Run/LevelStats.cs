using Unity.Entities;

partial struct LevelStats : IComponentData
{
    public uint Deaths;
    public double BestTime;
}