using Unity.Entities;

public class LevelAuthoring : UnityEngine.MonoBehaviour
{
    public float MouseSensitivity;
    public float ThrustWindup = 0.5f;
    public float ThrustDuration = 0.5f;
    public float ThrustCooldown = 5f;
    public float ThrustForce = 10f;

    public float TurnSmallTransitionIn = 0.1f;
    public float TurnSmallDuration = 31f/24f;

    public float AfterThrustTransition = 1f;

    public float IncreasedDragDuringFlip = 10f;

    public UnityEngine.GameObject CubePrefab;
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
                , TurnSmallTransitionIn = authoring.TurnSmallTransitionIn
                , TurnSmallDuration = authoring.TurnSmallDuration
                , AfterThrustTransition = authoring.AfterThrustTransition
                , IncreasedDragDuringFlip = authoring.IncreasedDragDuringFlip
                , CubePrefab = GetEntity(authoring.CubePrefab)
                }
            );
    }
}