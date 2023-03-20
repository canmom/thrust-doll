using Unity.Entities;
using Unity.Transforms;
using Latios;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial class CameraUpdateSystem : SubSystem
{
    private UnityEngine.Transform _camera;
    private UnityEngine.Transform _raymarchingQuad;

    protected override void OnCreate() {
        _camera = UnityEngine.Camera.main.transform;
        var raymarchingQuadGO = UnityEngine.GameObject.Find("RaymarchingQuad");
        if (raymarchingQuadGO is not null) {
            _raymarchingQuad = raymarchingQuadGO.transform;
        }

        RequireForUpdate<CameraTransform>();
    }

    protected override void OnUpdate() {
        Entity cameraTransform =
            SystemAPI
                .GetSingletonEntity<CameraTransform>();

        LocalToWorld transform =
            SystemAPI
                .GetComponent<LocalToWorld>(cameraTransform);

        _camera.position = transform.Position;
        _camera.rotation = transform.Rotation;

        if (_raymarchingQuad is not null)
        {
            _raymarchingQuad.position = transform.Position + transform.Forward;
            _raymarchingQuad.rotation = transform.Rotation;
        }
    }
}