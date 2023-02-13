using Unity.Entities;

partial struct Level : IComponentData
{
    public float MouseSensitivity;

    public float ThrustDuration;
    public float ThrustCooldown;
    
}