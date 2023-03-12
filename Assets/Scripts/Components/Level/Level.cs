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
}