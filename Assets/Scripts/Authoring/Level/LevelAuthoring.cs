using Unity.Entities;

public class LevelAuthoring : UnityEngine.MonoBehaviour
{
    public float MouseSensitivity;
    public float ThrustWindup = 0.3f;
    public float ThrustDuration = 0.5f;
    public float ThrustCooldown = 5f;
    public float ThrustForce = 10f;
}

public class LevelBaker : Baker<LevelAuthoring>
{
    public override void Bake(LevelAuthoring authoring)
    {
        AddComponent
            ( new Level
                { MouseSensitivity = authoring.MouseSensitivity
                , ThrustWindup = authoring.ThrustWindup
                , ThrustDuration = authoring.ThrustDuration
                , ThrustCooldown = authoring.ThrustCooldown
                , ThrustForce = authoring.ThrustForce
                }
            );
    }
}