using Unity.Entities;

public class LevelAuthoring : UnityEngine.MonoBehaviour
{
    public float MouseSensitivity;
    public float ThrustDuration = 0.5f;
    public float ThrustCooldown = 5f;
}

public class LevelBaker : Baker<LevelAuthoring>
{
    public override void Bake(LevelAuthoring authoring)
    {
        AddComponent
            ( new Level
                { MouseSensitivity = authoring.MouseSensitivity
                , ThrustDuration = authoring.ThrustDuration
                , ThrustCooldown = authoring.ThrustCooldown
                }
            );
    }
}