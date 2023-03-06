using Unity.Entities;
using Unity.Jobs;
using Latios;
using Latios.Psyshock;

partial struct PlayerCollisionLayer : ICollectionComponent
{
    public CollisionLayer Layer;

    public ComponentType AssociatedComponentType => ComponentType.ReadWrite<PlayerCollisionLayerTag>();

    public JobHandle TryDispose(JobHandle inputDeps) => Layer.IsCreated ? Layer.Dispose(inputDeps) : inputDeps;
}

partial struct PlayerCollisionLayerTag : IComponentData
{

}