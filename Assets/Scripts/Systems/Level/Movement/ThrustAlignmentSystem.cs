using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(ThrustStartSystem))]
[BurstCompile]
partial struct ThrustAlignmentSystem : ISystem
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

        new ThrustRotationJob
            { Time = SystemAPI.Time.ElapsedTime
            , Duration = duration
            }
            .Schedule();

        new ThrustFlipJob
            { Time = SystemAPI.Time.ElapsedTime
            , Duration = duration
            }
            .Schedule();

    }
}

[WithAll(typeof(Thrust))]
partial struct ThrustRotationJob : IJobEntity
{
    public double Time;
    public float Duration;

    void Execute(in ThrustRotation windup, ref Rotation rotation)
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

[WithAll(typeof(Thrust))]
partial struct ThrustFlipJob : IJobEntity
{
    public double Time;
    public float Duration;

    void Execute(in ThrustFlip flip, ref Rotation rotation)
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