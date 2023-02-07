using Unity.Entities;

public class CameraTransformAuthoring : UnityEngine.MonoBehaviour
{
}

class CameraTransformBaker : Baker<CameraTransformAuthoring>
{
    public override void Bake(CameraTransformAuthoring authoring)
    {
        AddComponent<CameraTransform>();
    }
}
