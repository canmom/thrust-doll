using Unity.Entities;

public class CameraPivotAuthoring : UnityEngine.MonoBehaviour
{
}

class CameraPivotBaker : Baker<CameraPivotAuthoring>
{
    public override void Bake(CameraPivotAuthoring authoring)
    {
        AddComponent<CameraPivot>();
    }
}
