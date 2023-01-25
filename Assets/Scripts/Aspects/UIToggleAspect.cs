using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct UIToggleAspect : IAspect
{
    readonly RefRW<UIToggleRingThickness> RingThickness;
    readonly RefRW<UIToggleCentreOpacity> CentreOpacity;
    readonly RefRW<UIToggle> ToggleComponent;
    readonly RefRW<StateSpring> StateSpring;

    //readonly TransformAspect Transform;

    readonly RefRW<Translation> TranslationComponent;
    readonly RefRW<NonUniformScale> ScaleComponent;

    public float Thickness {
        get => RingThickness.ValueRO.Thickness;
        set => RingThickness.ValueRW.Thickness = value;
    }

    public float Opacity {
        get => CentreOpacity.ValueRO.Opacity;
        set => CentreOpacity.ValueRW.Opacity = value;
    }

    public Entity BelongsTo {
        get => ToggleComponent.ValueRO.BelongsTo;
    }

    public float Scale {
        set => ScaleComponent.ValueRW.Value = new float3(value);
    }

    public float3 Translation {
        get => TranslationComponent.ValueRO.Value;
        set => TranslationComponent.ValueRW.Value = value;
    }

    public float StateTarget {
        get => StateSpring.ValueRO.Target;
        set => StateSpring.ValueRW.Target = value;
    }

    public float StateDisplacement {
        get => StateSpring.ValueRO.Displacement;
    }
}