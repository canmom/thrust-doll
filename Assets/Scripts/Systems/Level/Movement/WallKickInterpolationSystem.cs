using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Latios;

[BurstCompile]
partial struct WallKickInterpolationSystem : ISystem
{
    LatiosWorldUnmanaged _latiosWorld;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _latiosWorld = state.GetLatiosWorldUnmanaged();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Level level =
            _latiosWorld
                .sceneBlackboardEntity
                .GetComponentData<Level>();

        EntityCommandBuffer ecb =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

        new WallKickLerpJob
            { Time = SystemAPI.Time.ElapsedTime
            , NotionalDuration = level.WallkickFacingDuration
            , ECB = ecb
            }
            .Schedule();
    }
}

partial struct WallKickLerpJob : IJobEntity
{
    public double Time;

    public float NotionalDuration;

    public EntityCommandBuffer ECB;

    void Execute
        ( in SDFCollision collision
        , in CollidesWithSDF collider
        , in FaceWall faceWall
        , ref Velocity velocity
        , ref Rotation rotation
        , ref AnimationClipTimeOverride timeOverride
        , ref CurrentAnimationClip clip
        , Entity entity
        )
    {
        float tau =
            1f
            - (collision.Distance - collider.InnerRadius)
            / (   collider.Radius - collider.InnerRadius);

        quaternion targetRotation =
            quaternion
                .LookRotationSafe
                    ( velocity.Value
                    , collision.Normal
                    );

        rotation.Value =
            math
                .slerp
                    ( faceWall.InitialRotation
                    , targetRotation
                    , tau
                    );

        timeOverride.ClipTime =
            tau * NotionalDuration;

        //inner collider
        if (tau > 1f) {
            float NotionalStartTime = (float) (Time - NotionalDuration);

            clip.Start = NotionalStartTime;

            float3 targetVelocity =
                velocity.Value - 2 * math.dot(velocity.Value, collision.Normal) * collision.Normal;

            quaternion finalTargetRotation =
                quaternion
                    .LookRotationSafe
                        ( targetVelocity
                        , collision.Normal
                        );

            ECB.RemoveComponent<FaceWall>(entity);
            ECB.RemoveComponent<AnimationClipTimeOverride>(entity);
            ECB.AddComponent
                ( entity
                , new RotateTo
                    { InitialRotation = rotation.Value
                    , TargetRotation = finalTargetRotation
                    , TimeCreated = Time
                    }
                );
            ECB.AddComponent
                ( entity
                , new WallKick
                    { IncidentVelocity = velocity.Value
                    , ReflectionVelocity = targetVelocity
                    , Normal = collision.Normal
                    , TimeCreated = Time
                    }
                );

            velocity.Value = new float3(0f);
        }
    }
}

[WithAll(typeof(FaceWall),typeof(AnimationClipTimeOverride))]
[WithNone(typeof(SDFCollision))]
partial struct WallKickCancelJob : IJobEntity
{
    public EntityCommandBuffer ECB;

    void Execute(Entity entity)
    {
        ECB.RemoveComponent<FaceWall>(entity);
        ECB.RemoveComponent<AnimationClipTimeOverride>(entity);
    }
}