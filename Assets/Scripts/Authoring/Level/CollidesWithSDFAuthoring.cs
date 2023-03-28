using Unity.Entities;

public class CollidesWithSDFAuthoring : UnityEngine.MonoBehaviour
{
    public float CollisionRadius;
    public float InnerRadius;
}

public class CollidesWithSDFBaker : Baker<CollidesWithSDFAuthoring>
{
    public override void Bake(CollidesWithSDFAuthoring authoring)
    {
        Entity entity = GetEntity();

        AddComponent
            ( new CollidesWithSDF
                { Radius = authoring.CollisionRadius
                , InnerRadius = authoring.InnerRadius
                }
            );

        AddComponent<SDFCollision>();
        SetComponentEnabled<SDFCollision>(entity, false);
    }
}