using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using Latios;
using Latios.Psyshock;
using Unity.Transforms;

[UpdateInGroup(typeof(ConfiguratorSystemGroup))]
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

        _query = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Collider, Upgrade>()
            .Build(this);

        EntityManager.AddComponent<UISingleton>(SystemHandle);
    }

    protected override void OnUpdate()
    {
        Dependency = Physics.BuildCollisionLayer(_query, this).ScheduleParallel(out var collisionLayer, Allocator.TempJob, Dependency);

        var mouse = Mouse.current;

        UISingleton uiSingleton =
            SystemAPI
                .GetComponent<UISingleton>(SystemHandle);

        Entity currentHover =
            uiSingleton
                .CurrentHover;

        UnityEngine.Vector2 screenPosition =
            mouse
                .position
                .ReadValue();

        float delta =
            mouse
                .delta
                .x
                .ReadValue();

        quaternion deltaRotation =
            quaternion
                .RotateY(delta*-0.005f);

        UnityEngine.Ray ray =
            _camera
                .ScreenPointToRay(screenPosition);

        Dependency.Complete();

        if (mouse.leftButton.isPressed) {
            foreach 
                ( RefRW<Rotation> dollRotation
                    in
                        SystemAPI
                        .Query<RefRW<Rotation>>()
                        .WithAll<Character>()
                ) {

                    dollRotation.ValueRW.Value =
                        math.mul
                            ( deltaRotation
                            , dollRotation.ValueRO.Value
                            );
                }
        }
        
        bool didHit =
            Physics
                .Raycast
                    ( ray.origin
                    , ray.GetPoint(_camera.farClipPlane)
                    , collisionLayer
                    , out RaycastResult result
                    , out LayerBodyInfo hit
                    );

        if (didHit && hit.entity != currentHover) {

            //if we have moved from hovering over one target to another
            if (SystemAPI.Exists(currentHover)) {
                SystemAPI
                    .SetComponentEnabled<Hovering>
                        ( currentHover
                        , false
                        );
            }

            SystemAPI
                .SetComponentEnabled<Hovering>
                    ( hit.entity
                    , true
                    );

            SystemAPI
                .SetComponent<UISingleton>
                    ( SystemHandle
                    , new UISingleton
                        { CurrentHover = hit.entity
                        }
                    );

        } else if (!didHit) {
            if (SystemAPI.Exists(currentHover)) {
                SystemAPI
                    .SetComponentEnabled<Hovering>
                        ( currentHover
                        ,false
                        );
                SystemAPI
                    .SetComponent<UISingleton>
                        ( SystemHandle
                        , new UISingleton
                            { CurrentHover = Entity.Null
                            }
                        );
            }
        }

        if (mouse.leftButton.wasPressedThisFrame) {
            if (SystemAPI.Exists(uiSingleton.CurrentHover)) {
                SystemAPI.
                    SetComponentEnabled<On>
                        ( uiSingleton.CurrentHover
                        , !SystemAPI.IsComponentEnabled<On>
                            ( uiSingleton.CurrentHover
                            )
                        );
            }
        }
    }
}