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
        AddComponent
            ( new CollidesWithSDF
                { Radius = authoring.CollisionRadius
                , InnerRadius = authoring.InnerRadius
                }
            );
    }
}