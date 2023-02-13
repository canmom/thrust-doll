using Unity.Entities;
using Unity.Mathematics;

public class ReticuleAuthoring : UnityEngine.MonoBehaviour
{
    public float InitialAmount = 1.0f;
    public float4 InitialDotColour = new float4(1.0f);
}

public class ReticuleBaker : Baker<ReticuleAuthoring>
{
    public override void Bake(ReticuleAuthoring authoring)
    {
        AddComponent
            (
                new ReticuleCooldownRing
                    { Amount = authoring.InitialAmount
                    }
            );
        AddComponent
            (
                new ReticuleColour
                    { Colour = authoring.InitialDotColour
                    }
            );
    }
}