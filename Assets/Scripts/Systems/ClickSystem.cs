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
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var inputBuffer in SystemAPI.Query<DynamicBuffer<UIClick>>())
        {
            foreach(var uiClick in inputBuffer)
            {
                if(physicsWorld.CastRay(uiClick.Value,out var hit))
                {
                    var uiToggleEntity = SystemAPI.GetComponent<Upgrade>(hit.Entity).UIToggle;

                    UIToggleState oldState = SystemAPI.GetComponent<UIToggleState>(uiToggleEntity);

                    oldState.On = !oldState.On;
                    
                    SystemAPI.SetComponent<UIToggleState>(uiToggleEntity, oldState);
                }
            }
            inputBuffer.Clear();
        }
    }
}