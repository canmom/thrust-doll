using Unity.Entities;

public class WallColliderAuthoring : UnityEngine.MonoBehaviour
{
    //gameobject fields
}

public class WallColliderBaker : Baker<WallColliderAuthoring>
{
    public override void Bake(WallColliderAuthoring authoring)
    {
        AddComponent<WallCollider>();
    }
}