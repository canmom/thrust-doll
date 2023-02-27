using Unity.Assertions;
using Unity.Entities;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;

[BurstCompile]
public partial struct StatusEndJob<T> : IJobChunk
    where T : unmanaged, IComponentData, IStatus
{
    public double Time;
    public double Duration;
    public EntityCommandBuffer.ParallelWriter ECB;

    [ReadOnly] public ComponentTypeHandle<T> StatusHandle;
    public EntityTypeHandle EntityHandle;

    public void Execute
        ( in ArchetypeChunk chunk
        , int unfilteredChunkIndex
        , bool useEnabledMask
        , in v128 chunkEnabledMask
        )
    {
        //not usable with enableable components
        Assert.IsFalse(useEnabledMask);

        NativeArray<T> statuses =
            chunk
                .GetNativeArray<T>
                    ( ref StatusHandle
                    );
        NativeArray<Entity> entities =
            chunk
                .GetNativeArray
                    ( EntityHandle
                    );

        for (int i = 0; i < chunk.Count; ++i)
        {   
            T status = statuses[i];

            double timeElapsed = Time - status.TimeCreated;

            if (timeElapsed > Duration) {
                ECB.RemoveComponent<T>
                    ( unfilteredChunkIndex
                    , entities[i]
                    );
            }
        }
    }
}