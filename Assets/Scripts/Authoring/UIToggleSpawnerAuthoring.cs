using Unity.Entities;

class UIToggleSpawnerAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject UITogglePrefab;
}

class UIToggleSpawnerBaker : Baker<UIToggleSpawnerAuthoring>
{
    public override void Bake(UIToggleSpawnerAuthoring authoring)
    {
        AddComponent(new UIToggleSpawner
        {
            UITogglePrefab = GetEntity(authoring.UITogglePrefab)
        });
    }
}

