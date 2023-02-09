using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

[UpdateAfter(typeof(InputReadingSystem))]
[BurstCompile]
partial struct CameraControllerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state
            .EntityManager
            .AddComponent<CameraController>
                (state.SystemHandle
                );

        SystemAPI
            .SetComponent<CameraController>
                ( state.SystemHandle
                , new CameraController { Pitch = 0f, Yaw = 0f, Distance = 5f}
                );

        state
            .RequireForUpdate<Intent>();
        state
            .RequireForUpdate<CameraPivot>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float2 intendedRotation =
            SystemAPI
                .GetSingleton<Intent>()
                .Rotate;

        RefRW<CameraController> controller =
            SystemAPI
                .GetComponentRW<CameraController>
                    ( state.SystemHandle
                    );

        RefRW<Rotation> pivot =
            SystemAPI
                .GetComponentLookup<Rotation>()
                .GetRefRW
                    ( SystemAPI.GetSingletonEntity<CameraPivot>()
                    , false
                    );

        controller.ValueRW.Yaw
            += intendedRotation.x;

        controller.ValueRW.Pitch
            -= intendedRotation.y;

        controller.ValueRW.Pitch
            = math.clamp
                ( controller.ValueRO.Pitch
                , -math.PI/2
                , math.PI/2
                );

        pivot.ValueRW.Value =
            quaternion
                .EulerXYZ
                    ( controller.ValueRO.Pitch
                    , controller.ValueRO.Yaw
                    , 0f
                    );
    }
}