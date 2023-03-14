using Unity.Entities;
using Unity.Jobs;
using Latios;
using Latios.Psyshock;

partial struct WallCollisionLayer : ICollectionComponent
{
    public CollisionLayer Layer;

    public ComponentType AssociatedComponentType => ComponentType.ReadWrite<WallCollisionLayerTag>();

    public JobHandle TryDispose(JobHandle inputDeps) => Layer.IsCreated ? Layer.Dispose(inputDeps) : inputDeps;
}

partial struct WallCollisionLayerTag : IComponentData
{

}