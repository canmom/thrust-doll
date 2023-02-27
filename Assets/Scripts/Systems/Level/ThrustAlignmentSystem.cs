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

        new ThrustAlignmentJob
            { Time = SystemAPI.Time.ElapsedTime
            , Duration = duration
            }
            .Schedule();

    }
}

partial struct ThrustAlignmentJob : IJobEntity
{
    public double Time;
    public float Duration;

    void Execute(in Thrust thrust, in ThrustWindup windup, ref Rotation rotation)
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