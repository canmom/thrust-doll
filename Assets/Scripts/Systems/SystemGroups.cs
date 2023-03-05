using Unity.Entities;
using Unity.Transforms;
using Latios;

[UpdateBefore(typeof(TransformSystemGroup))]
public class ConfiguratorSystemGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        RequireForUpdate<Configurator>();
        base.OnCreate();
    }
}

[UpdateBefore(typeof(TransformSystemGroup))]
public class LevelSystemGroup : SuperSystem
{
    protected override void CreateSystems()
    {
        RequireForUpdate<Level>();
    }
}