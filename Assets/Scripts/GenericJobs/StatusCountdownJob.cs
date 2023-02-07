using Unity.Assertions;
using Unity.Entities;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;

[BurstCompile]
public partial struct StatusCountdownJob<T> : IJobChunk
    where T : struct, IComponentData, IStatus
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter ECB;

    ComponentTypeHandle<T> StatusHandle;
    EntityTypeHandle EntityHandle;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask) {
        //not usable with enableable components
        Assert.IsFalse(useEnabledMask);

        NativeArray<T> statuses = chunk.GetNativeArray<T>(ref StatusHandle);
        NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);

        for (int i = 0; i < chunk.Count; ++i)
        {   
            T status = statuses[i];

            status.TimeRemaining -= DeltaTime;

            statuses[i] = status;

            if (status.TimeRemaining < 0) {
                ECB.RemoveComponent<T>(unfilteredChunkIndex, entities[i]);
            }
        }
    }
}