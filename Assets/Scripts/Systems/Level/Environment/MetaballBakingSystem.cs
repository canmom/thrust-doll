using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Latios;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
partial class MetaballBakingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityQuery query =
            SystemAPI
                .QueryBuilder()
                .WithAll<Translation, NonUniformScale, Metaball>()
                .Build();

        NativeArray<Translation> translations =
            query
                .ToComponentDataArray<Translation>(Allocator.Temp);

        NativeArray<NonUniformScale> scales =
            query
                .ToComponentDataArray<NonUniformScale>(Allocator.Temp);

        Vector4[] translationVectors = new Vector4[16];
        float[] radii = new float[16];

        int i = 0;
        foreach (Translation translation in translations)
        {
            translationVectors[i] = (Vector4) (new float4(-translation.Value, 1.0f));
            ++i;
        }

        i = 0;
        foreach (NonUniformScale scale in scales)
        {
            radii[i] = (scale.Value.x + scale.Value.y + scale.Value.z)/6f;
            ++i;
        }
        
        Shader.SetGlobalVectorArray("_MetaballTranslations", translationVectors);
        Shader.SetGlobalFloatArray("_MetaballRadii", radii);
        Shader.SetGlobalInteger("_NumMetaballs", translations.Length);
    }
}