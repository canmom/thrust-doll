using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;

[UpdateAfter(typeof(InputReadingSystem))]
public partial class CameraControllerSystem : SystemBase
{
    private Camera _camera;

    protected override void OnCreate()
    {
        EntityManager.AddComponent<CameraController>(SystemHandle);
        SystemAPI.SetComponent<CameraController>(SystemHandle, new CameraController { Orientation = quaternion.identity, Distance = 5f});

        _camera = Camera.main;
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate()
    {
        float2 intendedRotation = SystemAPI.GetSingleton<Intent>().Rotate;

        quaternion rotation = quaternion.EulerXZY(intendedRotation.y, intendedRotation.x, 0f);

        RefRW<CameraController> controller = SystemAPI.GetComponentRW<CameraController>(SystemHandle);

        quaternion newOrientation = math.mul(rotation, controller.ValueRO.Orientation);

        controller.ValueRW.Orientation = newOrientation;

        _camera.transform.localRotation = newOrientation;
    }
}