using Unity.Entities;
using Unity.Burst;
using Latios;

[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(InputReadingSystem))]
[BurstCompile]
partial class RunRestartSystem : SubSystem
{
    protected override void OnCreate()
    {
        RequireForUpdate<RunEnd>();
    }

    protected override void OnUpdate()
    {
        Intent intent = SystemAPI.GetSingleton<Intent>();

        Entity blackboard =
            latiosWorld
                .sceneBlackboardEntity;

        EntityCommandBuffer ecb =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

        if (intent.Thrust) {
            ecb.AddComponent<StartNewRun>(blackboard);
            ecb.RemoveComponent<RunEnd>(blackboard);

            GameEndScreen.Instance.HideDeathScreen();
        }
    }
}