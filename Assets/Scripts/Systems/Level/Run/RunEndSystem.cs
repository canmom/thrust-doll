using Unity.Entities;
using Unity.Burst;
using Latios;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(ProjectilePlayerCollisionSystem))]
partial class RunEndSystem : SubSystem
{
    protected override void OnCreate()
    {
        RequireForUpdate<Run>();
    }

    protected override void OnUpdate()
    {
        BlackboardEntity blackboard =
            latiosWorld
                .sceneBlackboardEntity;

        EntityCommandBuffer ecb =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

        if (blackboard.HasComponent<RunEnd>())
        {
            UnityEngine.Debug.Log("Run ending in death.");

            RunEnd runEnd = blackboard.GetComponentData<RunEnd>();

            LevelStats stats = blackboard.GetComponentData<LevelStats>();

            LevelStats newStats =
                new LevelStats
                    { Deaths = ++stats.Deaths
                    , BestTime = stats.BestTime //do not change best time since we didn't win
                    };

            blackboard.SetComponentData( newStats );

            GameEndScreen.Instance.DisplayDeathScreen(runEnd.Duration, newStats.Deaths);

            ecb.RemoveComponent<Run>(blackboard);
        }
    }
}