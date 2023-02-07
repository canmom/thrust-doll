using Unity.Entities;

public struct ThrustCooldown : IStatus
{
    public float TimeRemaining {get; set;}

    public float InverseDuration;
}