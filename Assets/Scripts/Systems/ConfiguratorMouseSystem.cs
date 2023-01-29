using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using Latios;
using Latios.Psyshock;
using Unity.Transforms;

[UpdateAfter(typeof(SingleClipSystem))]
public partial class ConfiguratorMouseSystem : SubSystem
{
    private UnityEngine.Camera _camera;
    private EntityQuery _query;

    protected override void OnCreate()
    {
        if (_camera == null)
        {
            _camera = UnityEngine.Camera.main;
        }

        _query = new EntityQueryBuilder(Allocator.TempJob)
            .WithAll<Collider, Upgrade>()
            .Build(this);

        CheckedStateRef.EntityManager.AddComponent<UISingleton>(CheckedStateRef.SystemHandle);
    }

    protected override void OnUpdate()
    {
        var mouse = Mouse.current;
        UISingleton uiSingleton = SystemAPI.GetComponent<UISingleton>(CheckedStateRef.SystemHandle);

        var jobHandle = Physics.BuildCollisionLayer(_query, this).ScheduleParallel(out var collisionLayer, Allocator.TempJob);

        jobHandle.Complete();

        UnityEngine.Vector2 screenPosition = mouse.position.ReadValue();
        UnityEngine.Ray ray = _camera.ScreenPointToRay(screenPosition);

        Entity currentHover = uiSingleton.CurrentHover;
        bool didHit = Physics.Raycast(ray.origin, ray.GetPoint(_camera.farClipPlane), collisionLayer, out RaycastResult result, out LayerBodyInfo hit);
        if (didHit && hit.entity != currentHover) {
            //if we have moved from hovering over one target to another
            if (SystemAPI.Exists(currentHover)) {
                SystemAPI.SetComponentEnabled<Hovering>(currentHover,false);
            }
            SystemAPI.SetComponentEnabled<Hovering>(hit.entity,true);
            SystemAPI.SetComponent<UISingleton>(CheckedStateRef.SystemHandle, new UISingleton { CurrentHover = hit.entity });
        } else if (!didHit) {
            if (SystemAPI.Exists(currentHover)) {
                SystemAPI.SetComponentEnabled<Hovering>(currentHover,false);
                SystemAPI.SetComponent<UISingleton>(CheckedStateRef.SystemHandle, new UISingleton { CurrentHover = Entity.Null });
            }
        }

        if (mouse.leftButton.wasPressedThisFrame) {
            if (SystemAPI.Exists(uiSingleton.CurrentHover)) {
                SystemAPI.SetComponentEnabled<On>(uiSingleton.CurrentHover,!SystemAPI.IsComponentEnabled<On>(uiSingleton.CurrentHover));
            }
        }

        if (mouse.leftButton.isPressed) {
            float delta = mouse.delta.x.ReadValue();
            quaternion deltaRotation = quaternion.RotateY(delta*-0.005f);

            foreach (RefRW<Rotation> dollRotation in SystemAPI.Query<RefRW<Rotation>>().WithAll<Character>()) {
                dollRotation.ValueRW.Value = math.mul(deltaRotation,dollRotation.ValueRO.Value);
            }
        }
    }
}