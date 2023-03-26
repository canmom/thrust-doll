using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Latios;

[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(VelocitySystem))]
[BurstCompile]
partial struct SDFCollisionSystem : ISystem
{
    LatiosWorldUnmanaged _latiosWorld;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _latiosWorld = state.GetLatiosWorldUnmanaged();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Level level =
            _latiosWorld
                .sceneBlackboardEntity
                .GetComponentData<Level>();

        var MetaballsQuery =
            SystemAPI
                .QueryBuilder()
                .WithAll<Metaball,Translation>()
                .Build();

        NativeArray<Metaball> MetaballRadii =
            MetaballsQuery
                .ToComponentDataArray<Metaball>(Allocator.TempJob);

        NativeArray<Translation> MetaballTranslations =
            MetaballsQuery
                .ToComponentDataArray<Translation>(Allocator.TempJob);

        EntityCommandBuffer.ParallelWriter ecb =
            SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .AsParallelWriter();

        new SDFCollisionJob
            { MetaballRadii = MetaballRadii
            , MetaballTranslations = MetaballTranslations
            , Smooth = level.MetaballSmoothing
            , ECB = ecb
            }
            .ScheduleParallel();
    }
}

partial struct SDFCollisionJob : IJobEntity
{
    [ReadOnly] public NativeArray<Metaball> MetaballRadii;
    [ReadOnly] public NativeArray<Translation> MetaballTranslations;
    public float Smooth;

    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute
        ( [ChunkIndexInQuery] int chunkIndex
        , Entity entity
        , in Translation translation
        , in CollidesWithSDF collider
        )
    {
        float expDistance = 0;
        float4 normalExpDists = new float4 (0);

        float epsilon = 0.0001f;
        float2 k = new float2(1, -1);

        for (int i = 0; i < MetaballRadii.Length; ++i) {
            float metaballRadius =
                MetaballRadii[i].Radius;
            float3 metaballTranslation =
                MetaballTranslations[i].Value;
            float3 pos =
                translation.Value;

            float sphereDist =
                sphereSDF
                    ( pos
                    , metaballTranslation
                    , metaballRadius
                    );

            float4 normalDists =
                new float4
                    ( sphereSDF
                        ( pos + k.xyy * epsilon
                        , metaballTranslation
                        , metaballRadius
                        )
                    , sphereSDF
                        ( pos + k.yyx * epsilon
                        , metaballTranslation
                        , metaballRadius
                        )
                    , sphereSDF
                        ( pos + k.yxy * epsilon
                        , metaballTranslation
                        , metaballRadius
                        )
                    , sphereSDF
                        ( pos + k.xxx * epsilon
                        , metaballTranslation
                        , metaballRadius
                        )
                    );

            expDistance += math.exp(-sphereDist * Smooth);

            normalExpDists += math.exp(- normalDists * Smooth);
        }

        float distance = math.log(expDistance);

        float3 normal =
            math.normalize
                ( k.xyy * normalExpDists.x
                + k.yyx * normalExpDists.y
                + k.yxy * normalExpDists.z
                + k.xxx * normalExpDists.w
                );

        if (distance < collider.Radius) {
            ECB.AddComponent
                ( chunkIndex
                , entity
                , new SDFCollision
                    { Normal = normal
                    }
                );
        } else {
            ECB.RemoveComponent<SDFCollision>
                ( chunkIndex
                , entity);
        }
    }

    float sphereSDF(float3 pos, float3 spherePos, float sphereRadius)
    {
        return math.length(pos - spherePos) - sphereRadius;
    }
}