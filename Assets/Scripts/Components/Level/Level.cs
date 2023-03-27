using Unity.Entities;

partial struct Level : IComponentData
{
    public float MouseSensitivity;

    public float ThrustWindup;
    public float ThrustDuration;
    public float ThrustCooldown;
    public float ThrustForce;
    
    public float TurnSmallTransitionIn;
    public float TurnSmallDuration;

    public float AfterThrustTransition;

    public float WallkickFacingDuration;
    public float WallkickStopDuration;

    public float IncreasedDragDuringFlip;

    public Entity CubePrefab;

    public float MetaballSmoothing;
}