using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[BurstCompile]
partial struct ClickSystem : ISystem
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
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach(var input in SystemAPI.Query<DynamicBuffer<UIClick>>())
        {
            foreach(var placementInput in input)
            {
                if(physicsWorld.CastRay(placementInput.Value,out var hit))
                {
                    Debug.Log($"{hit.Position}");
                }
            }
            input.Clear();
        }
    }
}