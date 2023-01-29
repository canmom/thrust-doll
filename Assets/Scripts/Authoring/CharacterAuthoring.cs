using Unity.Entities;

public class CharacterAuthoring : UnityEngine.MonoBehaviour
{
}

class CharacterBaker : Baker<CharacterAuthoring>
{
    public override void Bake(CharacterAuthoring authoring)
    {
        AddComponent<Character>();
    }
}
