using Unity.Entities;

public class MetaballAuthoring : UnityEngine.MonoBehaviour
{
}

public class MetaballBaker : Baker<MetaballAuthoring>
{
    public override void Bake(MetaballAuthoring authoring)
    {
        AddComponent<Metaball>();
    }
}