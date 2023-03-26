using Unity.Entities;
using Unity.Rendering;

public class MetaballAuthoring : UnityEngine.MonoBehaviour
{
}

public class MetaballBaker : Baker<MetaballAuthoring>
{
    public override void Bake(MetaballAuthoring authoring)
    {
        float radius =
            ( authoring.transform.localScale.x
            + authoring.transform.localScale.y
            + authoring.transform.localScale.z
            ) / 6f;
        AddComponent
            ( new Metaball
                { Radius = radius
                }
            );
        AddComponent<DisableRendering>();
    }
}