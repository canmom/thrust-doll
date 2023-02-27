using Unity.Entities;

public struct ThrustCooldown : IStatus
{
    public double TimeCreated {get; set;}

    public double InverseDuration;
}