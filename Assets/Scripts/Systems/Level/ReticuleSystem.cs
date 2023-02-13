using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(StatusTickSystem))]
partial struct ReticuleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ComponentLookup<ThrustCooldown> thrustCooldownLookup =
            SystemAPI
                .GetComponentLookup<ThrustCooldown>();

        Entity playerEntity =
            SystemAPI
                .GetSingletonEntity<Character>();

        new ReticuleJob
            { ThrustCooldownLookup = thrustCooldownLookup
            , PlayerEntity = playerEntity
            }.Schedule();
    }
}

partial struct ReticuleJob : IJobEntity
{
    public ComponentLookup<ThrustCooldown> ThrustCooldownLookup;
    public Entity PlayerEntity;

    void Execute(ref ReticuleCooldownRing cooldownRing, ref ReticuleColour dotColour)
    {
        if ( ThrustCooldownLookup.TryGetComponent(PlayerEntity, out ThrustCooldown cooldown))
        {
            cooldownRing.Amount = cooldown.TimeRemaining * cooldown.InverseDuration;
            dotColour.Colour = new float4(1f, 0f, 0f, 0.5f);
        } else {
            cooldownRing.Amount = 0f;
            dotColour.Colour = new float4(1f);
        }
    }
}