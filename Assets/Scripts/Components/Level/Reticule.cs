using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

[MaterialProperty("_Amount")]
partial struct ReticuleCooldownRing : IComponentData
{
    public float Amount;
}

[MaterialProperty("_CentreDotColour")]
partial struct ReticuleColour : IComponentData
{
    public float4 Colour;
}
