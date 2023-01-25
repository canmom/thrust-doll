// using UnityEngine;
using Unity.Burst;
// using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[BurstCompile]
partial struct MouseHandlingSystem : ISystem
{   
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.AddComponent<UISingleton>(state.SystemHandle);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        UISingleton uiSingleton = SystemAPI.GetComponent<UISingleton>(state.SystemHandle);

        foreach(var inputBuffer in SystemAPI.Query<DynamicBuffer<MouseMove>>())
        {
            foreach(var mouseMove in inputBuffer)
            {
                Entity currentHover = uiSingleton.CurrentHover;
                bool hitConfirmed = physicsWorld.CastRay(mouseMove.Ray,out var hit);
                if (hitConfirmed && hit.Entity != currentHover) {
                    //if we have moved from hovering over one target to another
                    if (SystemAPI.Exists(currentHover)) {
                        SystemAPI.SetComponentEnabled<Hovering>(currentHover,false);
                    }
                    SystemAPI.SetComponentEnabled<Hovering>(hit.Entity,true);
                    SystemAPI.SetComponent<UISingleton>(state.SystemHandle, new UISingleton { CurrentHover = hit.Entity });
                } else if (!hitConfirmed) {
                    if (SystemAPI.Exists(currentHover)) {
                        SystemAPI.SetComponentEnabled<Hovering>(currentHover,false);
                        SystemAPI.SetComponent<UISingleton>(state.SystemHandle, new UISingleton { CurrentHover = Entity.Null });
                    }
                }
            }
            inputBuffer.Clear();
        }
        foreach(var inputBuffer in SystemAPI.Query<DynamicBuffer<MouseClick>>())
        {
            foreach(var mouseClick in inputBuffer)
            {
                if (SystemAPI.Exists(uiSingleton.CurrentHover)) {
                    SystemAPI.SetComponentEnabled<On>(uiSingleton.CurrentHover,!SystemAPI.IsComponentEnabled<On>(uiSingleton.CurrentHover));
                }
            }
            inputBuffer.Clear();
        }
    }
}