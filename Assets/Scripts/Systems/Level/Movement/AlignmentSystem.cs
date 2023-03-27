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

        new RotateToJob
            { Time = SystemAPI.Time.ElapsedTime
            , Duration = duration
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
    public float Duration;

    void Execute(in RotateTo windup, ref Rotation rotation)
    {
        float amount = 
            ThrustDoll
                .Util
                .BezierComponent
                    ( 0f
                    , 1.1f
                    , (float)(Time - windup.TimeCreated) / Duration
                    );
        
        rotation.Value =
            math
                .slerp
                    ( windup.InitialRotation
                    , windup.TargetRotation
                    , amount
                    );
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