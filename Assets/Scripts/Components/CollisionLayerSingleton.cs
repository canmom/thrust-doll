using Unity.Entities;
using Latios.Psyshock;
using Unity.Jobs;

struct CollisionLayerSingleton : IComponentData
{
    public CollisionLayer CollisionLayer;
    public JobHandle Job;
}