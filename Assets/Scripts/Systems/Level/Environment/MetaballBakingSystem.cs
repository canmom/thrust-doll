using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
partial class MetaballBakingSystem : BakingSystem
{
    protected override void OnUpdate()
    {
        EntityQuery query =
            SystemAPI
                .QueryBuilder()
                .WithAll<Translation, Metaball>()
                .Build();

        NativeArray<Translation> transforms =
            query
                .ToComponentDataArray<LocalToWorld>(Allocator.Temp);

        Matrix4x4[] transformMatrices = new Matrix4x4[16];

        int i = 0;
        foreach (LocalToWorld transform in transforms)
        {

            transformMatrices[i] = (Matrix4x4) math.inverse(transform.Value);
            ++i;
        }
        
        Shader.SetGlobalMatrixArray("_MetaballTransforms", transformMatrices);
    }
}