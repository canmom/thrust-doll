using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(StatusTransitionSystem))]
partial struct ReticuleSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state
            .RequireForUpdate<Character>();
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
            , Time = SystemAPI.Time.ElapsedTime
            }.Schedule();
    }
}

partial struct ReticuleJob : IJobEntity
{
    public ComponentLookup<ThrustCooldown> ThrustCooldownLookup;
    public Entity PlayerEntity;
    public double Time;

    void Execute(ref ReticuleCooldownRing cooldownRing, ref ReticuleColour dotColour)
    {
        if ( ThrustCooldownLookup.TryGetComponent(PlayerEntity, out ThrustCooldown cooldown))
        {
            cooldownRing.Amount = (float) ((Time - cooldown.TimeCreated) * cooldown.InverseDuration);
            dotColour.Colour = new float4(1f, 0f, 0f, 0.5f);
        } else {
            cooldownRing.Amount = 1f;
            dotColour.Colour = new float4(1f);
        }
    }
}