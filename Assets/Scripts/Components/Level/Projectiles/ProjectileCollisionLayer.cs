using Unity.Entities;
using Unity.Jobs;
using Latios;
using Latios.Psyshock;

partial struct ProjectileCollisionLayer : ICollectionComponent
{
    public CollisionLayer Layer;

    public ComponentType AssociatedComponentType => ComponentType.ReadWrite<ProjectileCollisionLayerTag>();

    public JobHandle TryDispose(JobHandle inputDeps) => Layer.IsCreated ? Layer.Dispose(inputDeps) : inputDeps;
}

partial struct ProjectileCollisionLayerTag : IComponentData
{

}