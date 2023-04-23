using Unity.Entities;

public class SpawnPointAuthoring : UnityEngine.MonoBehaviour
{
    //gameobject fields
}

public class SpawnPointBaker : Baker<SpawnPointAuthoring>
{
    public override void Bake(SpawnPointAuthoring authoring)
    {
        AddComponent<SpawnPoint>();
    }
}