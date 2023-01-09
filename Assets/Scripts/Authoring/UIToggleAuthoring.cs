using Unity.Entities;

public class UIToggleAuthoring : UnityEngine.MonoBehaviour
{
}

class UIToggleBaker : Baker<UIToggleAuthoring>
{
    public override void Bake(UIToggleAuthoring authoring)
    {
        AddComponent<UIToggleRingThickness>();
        AddComponent<UIToggleCentreOpacity>();
    }
}
