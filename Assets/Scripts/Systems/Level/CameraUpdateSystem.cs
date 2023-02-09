using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial class CameraUpdateSystem : SystemBase
{
    private UnityEngine.Camera _camera;

    protected override void OnCreate() {
        _camera = UnityEngine.Camera.main;

        RequireForUpdate<CameraTransform>();
    }

    protected override void OnUpdate() {
        Entity cameraTransform =
            SystemAPI
                .GetSingletonEntity<CameraTransform>();

        LocalToWorld transform =
            SystemAPI
                .GetComponent<LocalToWorld>(cameraTransform);

        _camera.transform.position = transform.Position;
        _camera.transform.rotation = transform.Rotation;
    }
}
        