using Unity.Entities;

partial struct ProjectileSpawner : IComponentData
{
    public double LastShot;
    public float Interval;
    public float ShotSpeed;
}