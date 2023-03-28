using Unity.Entities;
using Unity.Burst;
using Latios;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(LevelSystemGroup))]
[UpdateAfter(typeof(AngularVelocitySystem))]
partial struct ProjectileSpawningSystem : ISystem
{
    LatiosWorldUnmanaged _latiosWorld;
    NativeReference<Random> _random;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _latiosWorld = state.GetLatiosWorldUnmanaged();
        //set up random
        _random = new NativeReference<Random>(new Random(1234), Allocator.Persistent);
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

        var icb =
            _latiosWorld
                .syncPoint
                .CreateInstantiateCommandBuffer<Velocity, Translation, ProjectileShaderOffset>();

        new SpiralSpawningJob
            { Time = SystemAPI.Time.ElapsedTime
            , BulletPrefab = level.BulletPrefab
            , Rand = _random
            , ICB = icb
            }
            .Schedule();


    }
}

partial struct SpiralSpawningJob : IJobEntity
{
    public double Time;
    public Entity BulletPrefab;

    public NativeReference<Random> Rand;

    public InstantiateCommandBuffer<Velocity, Translation, ProjectileShaderOffset> ICB;

    void Execute
        ( ref ProjectileSpawner spawner
        , in Translation translation
        , in Rotation rotation
        , [ChunkIndexInQuery] int chunkIndex
        )
    {
        float sinceLastShot = (float) (Time - spawner.LastShot);

        if (sinceLastShot > spawner.Interval)
        {
            Random rand = Rand.Value;

            Velocity velocity =
                new Velocity
                    { Value =
                        math.mul(rotation.Value, new float3(0,0,1)) * spawner.ShotSpeed
                    };

            spawner.LastShot = Time;

            ProjectileShaderOffset offset =
                new ProjectileShaderOffset
                    { Offset = rand.NextFloat2()
                    };

            ICB.Add(BulletPrefab, velocity, translation, offset, chunkIndex);

            Rand.Value = rand;
        }
    }
}