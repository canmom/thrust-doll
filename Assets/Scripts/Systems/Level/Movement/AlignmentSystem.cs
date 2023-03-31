using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(ThrustStartSystem))]
[BurstCompile]
partial struct AlignmentSystem : ISystem
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
        float duration = SystemAPI.GetSingleton<Level>().ThrustWindup;

        EntityCommandBuffer ecb =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);        

        new RotateToJob
            { Time = SystemAPI.Time.ElapsedTime
            , ECB = ecb
            }
            .Schedule();

        new FlipJob
            { Time = SystemAPI.Time.ElapsedTime
            , Duration = duration
            }
            .Schedule();

    }
}

partial struct RotateToJob : IJobEntity
{
    public double Time;
    public EntityCommandBuffer ECB;

    void Execute(in RotateTo windup, ref Rotation rotation, Entity entity)
    {
        float tau = (float)(Time - windup.TimeCreated) / windup.Duration;

        float amount = 
            ThrustDoll
                .Util
                .BezierComponent
                    ( 0f
                    , 1.1f
                    , tau
                    );
        
        rotation.Value =
            math
                .slerp
                    ( windup.InitialRotation
                    , windup.TargetRotation
                    , amount
                    );

        if (tau > 1)
        {
            ECB.RemoveComponent<RotateTo>(entity);
        }
    }
}

partial struct FlipJob : IJobEntity
{
    public double Time;
    public float Duration;

    void Execute(in Flip flip, ref Rotation rotation)
    {
        float amount = 
            ThrustDoll
                .Util
                .BezierComponent
                    ( 0f
                    , 0.8f
                    , (float)(Time - flip.TimeCreated) / Duration
                    );

        quaternion flipRotation =
            math.mul
                ( flip
                    .InitialRotation
                , math.slerp
                    ( quaternion.identity
                    , quaternion.RotateX(math.PI)
                    , amount
                    )
                );

        rotation.Value =
            math.slerp
                ( flipRotation
                , flip.BackRotation
                , amount
                );
    }
}