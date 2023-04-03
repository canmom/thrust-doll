using Unity.Core;
using Unity.Entities;
using Unity.Transforms;
using Latios;
using Unity.Mathematics;

[UpdateBefore(typeof(TransformSystemGroup))]
public class ConfiguratorSystemGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        RequireForUpdate<Configurator>();
        base.OnCreate();
    }
}

[UpdateBefore(typeof(TransformSystemGroup))]
public partial class LevelSystemGroup : SuperSystem
{
    protected override void CreateSystems()
    {
        RequireForUpdate<Level>();
        //RequireForUpdate<Run>();
    }

    protected override void OnUpdate()
    {
        BlackboardEntity blackboard =
            latiosWorldUnmanaged
                .sceneBlackboardEntity;

        bool popTime = false;

        if ( blackboard.HasComponent<RunEnd>() )
        {
            popTime = true;

            double tEnd =
                blackboard
                    .GetComponentData<RunEnd>()
                    .Time;

            double T =
                blackboard
                    .GetComponentData<Level>()
                    .SlowMoEnd;

            double t = SystemAPI.Time.ElapsedTime;

            float dt = SystemAPI.Time.DeltaTime;

            double exponential = math.exp(-(t - tEnd)/T);

            double adjustedTime = tEnd + T * (1 - exponential);

            float adjustedDt = dt * (float) exponential;

            TimeData tempTime = new TimeData(adjustedTime, adjustedDt);

            World.PushTime(tempTime);
        }
        base.OnUpdate();

        if ( popTime )
        {
            World.PopTime();
        }
    }
}