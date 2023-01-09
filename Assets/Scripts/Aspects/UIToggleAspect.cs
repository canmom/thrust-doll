using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct UIToggleAspect : IAspect
{
    readonly RefRW<UIToggleRingThickness> RingThickness;
    readonly RefRW<UIToggleCentreOpacity> CentreOpacity;

    readonly TransformAspect Transform;

    public float Thickness {
        get => RingThickness.ValueRO.Thickness;
        set => RingThickness.ValueRW.Thickness = value;
    }

    public float Opacity {
        get => CentreOpacity.ValueRO.Opacity;
        set => CentreOpacity.ValueRW.Opacity = value;
    }

    public float Scale {
        get => Transform.LocalScale;
        set => Transform.LocalScale = value;
    }
}