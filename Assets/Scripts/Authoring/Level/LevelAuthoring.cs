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

    public float WallkickFacingDuration = 1f/3f;
    public float WallkickTransientDuration = 5f/24f;
    public float WallkickStopDuration = 0.1f;
    public float WallkickRealignmentDuration = 5f;

    public float IncreasedDragDuringFlip = 10f;

    public UnityEngine.GameObject CubePrefab;
    public UnityEngine.GameObject BulletPrefab;

    public float MetaballSmoothing = 1.2f;

    public double SlowMoEnd = 5;
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
                , WallkickFacingDuration = authoring.WallkickFacingDuration
                , WallkickTransientDuration = authoring.WallkickTransientDuration
                , WallkickStopDuration = authoring.WallkickStopDuration
                , WallkickRealignmentDuration = authoring.WallkickRealignmentDuration
                , IncreasedDragDuringFlip = authoring.IncreasedDragDuringFlip
                , CubePrefab = GetEntity(authoring.CubePrefab)
                , BulletPrefab = GetEntity(authoring.BulletPrefab)
                , MetaballSmoothing = authoring.MetaballSmoothing
                , SlowMoEnd = authoring.SlowMoEnd
                }
            );
    }
}