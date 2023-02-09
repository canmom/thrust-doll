using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;

[UpdateAfter(typeof(VelocitySystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
[BurstCompile]
partial struct CameraPivotTranslationSystem : ISystem
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
        ComponentLookup<Translation> translationLookup =
            SystemAPI
                .GetComponentLookup<Translation>(false);

        Entity characterEntity =
            SystemAPI
                .GetSingletonEntity<Character>();

        Entity pivotEntity =
            SystemAPI
                .GetSingletonEntity<CameraPivot>();


        var job = new CameraPivotTranslationJob
            { TranslationLookup = translationLookup
            ,   CharacterEntity = characterEntity
            ,       PivotEntity = pivotEntity
            };

        state.Dependency = job.Schedule(state.Dependency);
    }
}

[BurstCompile]
partial struct CameraPivotTranslationJob : IJob
{
    public ComponentLookup<Translation> TranslationLookup;
    public Entity CharacterEntity;
    public Entity PivotEntity;

    public void Execute()
    {
        TranslationLookup[PivotEntity] = TranslationLookup[CharacterEntity];
    }
}