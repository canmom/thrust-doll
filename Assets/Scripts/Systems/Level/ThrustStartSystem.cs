using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(CameraControllerSystem))]
[BurstCompile]
partial struct ThrustStartSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Character>();
        state.RequireForUpdate<CameraPivot>();
        state.RequireForUpdate<Intent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.GetSingleton<Intent>().Thrust)
        {   
            Entity player =
                SystemAPI
                    .GetSingletonEntity<Character>();

            if (!SystemAPI.HasComponent<ThrustCooldown>(player))
            {
                quaternion rotation =
                    SystemAPI
                        .GetComponent<Rotation>
                            ( SystemAPI
                                .GetSingletonEntity<CameraPivot>()
                            )
                        .Value;

                float3 acceleration = math.mul(rotation,new float3(0f, 0f, 10f ));

                EntityCommandBuffer ecb =
                    SystemAPI
                        .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                        .CreateCommandBuffer(state.WorldUnmanaged);

                ecb.AddComponent(player,
                    new Thrust
                        { TimeRemaining = 0.5f
                        , Acceleration = acceleration
                        });

                ecb.AddComponent(player,
                    new ThrustCooldown
                        { TimeRemaining = 5f
                        , InverseDuration = 0.2f
                        });
            }
        }
    }
}

