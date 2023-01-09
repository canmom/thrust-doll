using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject UITogglePrefab;

    public float StateSpringStiffness;
    public float StateSpringDamping;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new UIToggleSpawner
        {
            UITogglePrefab = GetEntity(authoring.UITogglePrefab)
        });

        AddComponent(new StateSpringConfig
        {
            Stiffness = authoring.StateSpringStiffness,
            Damping = authoring.StateSpringDamping
        });
    }
}

