using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class UpgradeAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
class UpgradeBaker : Baker<UpgradeAuthoring>
{
    public override void Bake(UpgradeAuthoring authoring)
    {
        AddComponent(new Upgrade {});
    }
}