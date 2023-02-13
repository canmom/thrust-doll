using Unity.Entities;
using Unity.Transforms;

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
public class LevelSystemGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        RequireForUpdate<Level>();
        base.OnCreate();
    }
}