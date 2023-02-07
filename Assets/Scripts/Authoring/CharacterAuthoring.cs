using Unity.Entities;
using Unity.Mathematics;

public class CharacterAuthoring : UnityEngine.MonoBehaviour
{
}

class CharacterBaker : Baker<CharacterAuthoring>
{
    public override void Bake(CharacterAuthoring authoring)
    {
        AddComponent<Character>();
        AddComponent(new Velocity { Value = new float3(0)});
    }
}
