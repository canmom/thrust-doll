using Unity.Entities;

public class ProjectileAuthoring : UnityEngine.MonoBehaviour
{
    //gameobject fields
}

public class ProjectileBaker : Baker<ProjectileAuthoring>
{
    public override void Bake(ProjectileAuthoring authoring)
    {
        AddComponent<Projectile>();
        AddComponent<Velocity>();
    }
}