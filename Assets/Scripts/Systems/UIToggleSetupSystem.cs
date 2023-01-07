using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
partial struct UIToggleSetupSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UIToggleSpawner>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //ECB boilerplate
        //We want it to run before the simulation starts, so use BeginSimulation buffer
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        //get the UIToggle prefab
        var prefab = SystemAPI.GetSingleton<UIToggleSpawner>().UITogglePrefab;

        foreach (var (upgradeRef, parentEntity) in SystemAPI.Query<RefRW<Upgrade>>().WithEntityAccess())
        {
            var instance = ecb.Instantiate(prefab);
            ecb.SetComponent(instance,LocalTransform.Identity);

            //define the coordinates to be relative to the parent; ParentSystem will take care of the rest.
            ecb.AddComponent(instance,new Parent{ Value = parentEntity });

            //store a reference to its UIToggle on the component.
            //IT IS VERY IMPORTANT THAT YOU USE THE ECB FOR THIS OR YOU'LL HAVE A NEGATIVE TEMPORARY ENTITY
            ecb.SetComponent<Upgrade>(parentEntity, new Upgrade { UIToggle = instance });
        }

        //This system should only run once.
        state.Enabled = false;
    }
}